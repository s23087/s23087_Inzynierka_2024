"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import getCurrencyValueByDate from "../flexible/get_chosen_currency_value";
import validators from "../validators/validator";

export default async function CreatePurchaseInvoice(
  products,
  file,
  orgs,
  state,
  formData,
) {
  let invoiceNumber = formData.get("invoice");
  let transport = formData.get("transport");
  let errorMessage = validateData(invoiceNumber, transport, products, file);

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  const dbName = await getDbName();
  let choosenUser = formData.get("user");
  let seller = formData.get("org");
  let invoiceDate = formData.get("date").replaceAll("-", "_");
  let fileName = `../../database/${dbName}/documents/${invoiceNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_")}_${choosenUser}${orgs.userOrgId}${seller}_${invoiceDate}${Date.now().toString()}.pdf`;
  let transformProducts = [];
  products.forEach((element) => {
    transformProducts.push({
      itemId: element.id,
      qty: element.qty,
      price: element.price,
    });
  });

  let chosenCurrency = formData.get("currency");
  let currencyExchangeDate = getExchangeDate(formData);
  let { euroVal, usdVal } = await getCurrencyValues(currencyExchangeDate);

  if (euroVal === null || usdVal === null) {
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }

  if (euroVal === 0 || usdVal === 0) {
    return {
      error: true,
      completed: true,
      message: "Could not download all currency values.",
    };
  }

  let purchaseInvoiceData = getData();

  const fs = require("node:fs");
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
  try {
    const userId = await getUserId();
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Invoices/add/purchase/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(purchaseInvoiceData),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.ok) {
      return {
        error: false,
        completed: true,
        message: "Success! You had created purchase invoice.",
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
  } catch {
    console.error("Create purchase invoice fetch failed.");
    try {
      fs.rmSync(fileName);
    } catch (error) {
      console.error(error);
      return {
        error: true,
        completed: true,
        message: "Connection error. Critical file error.",
      };
    }
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }

  function getData() {
    return {
      userId: parseInt(formData.get("user")),
      invoiceNumber: invoiceNumber,
      seller: parseInt(seller),
      buyer: orgs.userOrgId,
      invoiceDate: formData.get("date"),
      dueDate: formData.get("dueDate"),
      note: formData.get("description") ? formData.get("description") : "",
      inSystem: formData.get("status") === "true",
      transportCost: parseFloat(transport),
      invoiceFilePath: fileName,
      taxes: parseInt(formData.get("taxes")),
      currencyValueDate:
        chosenCurrency === "PLN"
          ? new Date().toLocaleDateString("en-CA")
          : currencyExchangeDate,
      currencyName: chosenCurrency,
      paymentMethodId: parseInt(formData.get("paymentMethod")),
      paymentsStatusId: parseInt(formData.get("paymentStatus")),
      euroValue: euroVal,
      usdValue: usdVal,
      invoiceItems: transformProducts,
    };
  }
}
async function getCurrencyValues(currencyExchangeDate) {
  let euroVal;
  let usdVal;
  let newDate = new Date(currencyExchangeDate);
  if (newDate.getDay() === 0 || newDate.getDay() === 6) {
    if (newDate.getDay() === 0) newDate.setDate(newDate.getDate() - 2); // If is sunday make it friday
    if (newDate.getDay() === 6) newDate.setDate(newDate.getDate() - 1); // If is saturday make it friday
    euroVal = await getCurrencyValueByDate(
      "EUR",
      newDate.toLocaleDateString("en-CA"),
    );
    usdVal = await getCurrencyValueByDate(
      "USD",
      newDate.toLocaleDateString("en-CA"),
    );
  } else {
    euroVal = await getCurrencyValueByDate("EUR", currencyExchangeDate);
    usdVal = await getCurrencyValueByDate("USD", currencyExchangeDate);
  }
  return { euroVal, usdVal };
}

function getExchangeDate(formData) {
  return formData.get("currencyExchange")
    ? formData.get("currencyExchange")
    : formData.get("date");
}

function validateData(invoiceNumber, transport, products, file) {
  let errorMessage = "Error:";
  if (
    !validators.lengthSmallerThen(invoiceNumber, 40) ||
    !validators.stringIsNotEmpty(invoiceNumber)
  )
    errorMessage += "\nInvoice must not be empty.";
  if (
    !validators.isPriceFormat(transport) ||
    !validators.stringIsNotEmpty(transport)
  )
    errorMessage += "\nTransport cost must not be empty and must be decimal.";
  if (products.length <= 0) errorMessage += "\nInvoice must have products.";
  if (!file) errorMessage += "\nDocument must not be empty.";
  return errorMessage;
}
