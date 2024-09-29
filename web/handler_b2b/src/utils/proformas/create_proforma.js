"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import getCurrencyValueByDate from "../flexible/get_chosen_currency_value";
import validators from "../validators/validator";

export default async function createProforma(
  products,
  file,
  orgs,
  isYourProforma,
  state,
  formData,
) {
  let errorMessage = "Error:";
  let chosenUser = formData.get("user");
  let proformaNumber = formData.get("proformaNumber");
  let seller = formData.get("org");
  let taxes = formData.get("taxes");
  let chosenCurrency = formData.get("currency");
  let transport = formData.get("transport");
  let paymentMethod = formData.get("paymentMethod");
  if (!paymentMethod) errorMessage += "\nPayment method must not be empty.";
  if (!chosenCurrency) errorMessage += "\nCurrency must not be empty.";
  if (!taxes) errorMessage += "\nTaxes must not be empty.";
  if (!seller) errorMessage += "\nClient must not be empty.";
  if (!chosenUser) errorMessage += "\nUser must not be empty.";
  if (
    !validators.lengthSmallerThen(proformaNumber, 40) ||
    !validators.stringIsNotEmpty(proformaNumber)
  )
    errorMessage += "\nProforma number must not be empty.";
  if (
    !validators.isPriceFormat(transport) ||
    !validators.stringIsNotEmpty(transport)
  )
    errorMessage += "\nTransport cost must not be empty and must be decimal.";
  if (products.length <= 0) errorMessage += "\nProforma must have products.";

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  const userId = await getUserId();
  const dbName = await getDbName();
  let proformaDate = formData.get("date").replaceAll("-", "_");
  let fileName = `../../database/${dbName}/documents/pr_${proformaNumber.replaceAll("/", "")}_${chosenUser}${orgs.userOrgId}${seller}_${proformaDate}${userId}${Date.now().toString()}.pdf`;
  let transformProducts = [];
  products.forEach((element) => {
    transformProducts.push({
      itemId: isYourProforma ? element.id : element.priceId,
      qty: element.qty,
      price: element.price,
    });
  });
  let currencyExchangeDate = formData.get("currencyExchange")
    ? formData.get("currencyExchange")
    : formData.get("date");
  let curVal = 1;
  let newDate = new Date(currencyExchangeDate);
  if (chosenCurrency !== "PLN") {
    if (newDate.getDay() === 0) newDate.setDate(newDate.getDate() - 2); // If is sunday make it friday
    if (newDate.getDay() === 6) newDate.setDate(newDate.getDate() - 1); // If is saturday make it friday
    curVal = await getCurrencyValueByDate(
      chosenCurrency,
      newDate.toLocaleDateString("en-CA"),
    );
  }

  if (curVal === 0) {
    return {
      error: true,
      completed: true,
      message: "Could not download currency value.",
    };
  }

  let proformaData = {
    isYourProforma: isYourProforma,
    proformaNumber: proformaNumber,
    sellerId: parseInt(seller),
    buyerId: orgs.userOrgId,
    date: formData.get("date"),
    transportCost: parseFloat(transport),
    note: formData.get("description") ? formData.get("description") : "",
    inSystem: formData.get("status") === "true",
    path: file ? fileName : null,
    taxId: parseInt(taxes),
    paymentId: parseInt(paymentMethod),
    currencyDate:
      chosenCurrency === "PLN"
        ? new Date().toLocaleDateString("en-CA")
        : currencyExchangeDate,
    currencyName: chosenCurrency,
    currencyValue: curVal,
    userId: parseInt(chosenUser),
    proformaItems: transformProducts,
  };

  const fs = require("node:fs");
  if (file) {
    try {
      if (fs.existsSync(fileName)) {
        return {
          error: true,
          completed: true,
          message: "That document already exist.",
        };
      }
      let buffArray = await file.get("file").arrayBuffer();
      let buff = new Uint8Array(buffArray);
      fs.writeFileSync(fileName, buff);
    } catch (error) {
      console.log(error);
      return {
        error: true,
        completed: true,
        message: "Could not upload file.",
      };
    }
  }

  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Proformas/add?userId=${userId}`,
    {
      method: "POST",
      body: JSON.stringify(proformaData),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );
  if (info.status === 404) {
    let text = await info.text();
    if (text === "User not found.") {
      logout();
      return {
        error: true,
        completed: true,
        message: "Your user profile does not exists.",
      };
    }
    return {
      error: true,
      completed: true,
      message: "Chosen user do not exists.",
    };
  }
  if (info.status === 400) {
    return {
      error: true,
      completed: true,
      message: await info.text(),
    };
  }
  if (info.status === 500) {
    return {
      error: true,
      completed: true,
      message: "Server error.",
    };
  }

  if (info.ok) {
    return {
      error: false,
      completed: true,
      message: "Success! You had created proforma.",
    };
  } else {
    try {
      fs.rmSync(fileName);
    } catch (error) {
      console.log(error);
      return {
        error: true,
        completed: true,
        message: "Critical file error.",
      };
    }
    return {
      error: true,
      completed: true,
      message: "Critical error.",
    };
  }
}
