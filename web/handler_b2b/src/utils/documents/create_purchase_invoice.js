"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import getCurrencyValueByDate from "../flexible/get_choosen_currency_value";
import validators from "../validators/validator";

export default async function CreatePurchaseInvoice(
  products,
  file,
  orgs,
  state,
  formData,
) {
  let message = "Error:";
  let invoice = formData.get("invoice");
  if (
    !validators.lengthSmallerThen(invoice, 40) ||
    !validators.stringIsNotEmpty(invoice)
  )
    message += "\nInvoice must not be empty.";
  let transport = formData.get("transport");
  if (
    !validators.isPriceFormat(transport) ||
    !validators.stringIsNotEmpty(transport)
  )
    message += "\nTransport cost must not be empty and must be decimal.";
  if (products.length <= 0) message += "\nInvoice must have products.";
  if (!file) message += "\nDocument must not be empty.";

  if (message.length > 6)
    return {
      error: true,
      completed: true,
      message: message,
    };

  const dbName = await getDbName();
  let choosenUser = formData.get("user");
  let org = formData.get("org");
  let date = formData.get("date").replaceAll("-", "_");
  let fileName = `../../database/${dbName}/documents/${invoice.replaceAll("/", "")}_${choosenUser}${orgs.userOrgId}${org}_${date}.pdf`;
  let transformProducts = [];
  products.forEach((element) => {
    transformProducts.push({
      itemId: element.id,
      qty: element.qty,
      price: element.price,
    });
  });

  let chosenCurrency = formData.get("currency");
  let currencyExchangeDate = formData.get("currencyExchange")
    ? formData.get("currencyExchange")
    : formData.get("date");
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

  if (euroVal === 0 || usdVal === 0) {
    return {
      error: true,
      completed: true,
      message: "Could not download all currency values.",
    };
  }

  let invoiceData = {
    userId: parseInt(formData.get("user")),
    invoiceNumber: invoice,
    seller: parseInt(org),
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

  const fs = require("node:fs");
  try {
    if (fs.existsSync(fileName)) {
      return {
        error: true,
        completed: true,
        message: "That document already exist.",
      };
    }
    let arrayBuffer = await file.get("file").arrayBuffer();
    let buffer = new Uint8Array(arrayBuffer);
    fs.writeFileSync(fileName, buffer);
  } catch (error) {
    console.log(error);
    return {
      error: true,
      completed: true,
      message: "Could not upload file.",
    };
  }

  const userId = await getUserId();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Invoices/addPurchaseInvoice?userId=${userId}`,
    {
      method: "POST",
      body: JSON.stringify(invoiceData),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  if (info.ok) {
    return {
      error: false,
      completed: true,
      message: "Success!",
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
