"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";

/**
 * Sends request to create sales invoice.
 * @param  {Array<{priceId: Number, id: Number, qty: Number, price: Number, invoiceId: Number}>} products Products added to invoice.
 * @param  {FormData} file FormData object containing file binary data.
 * @param  {{orgName: string, restOrgs: Array<{orgName: string, orgId: Number}>}} orgs Object that contain user organization name.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function CreateSalesInvoice(
  products,
  file,
  orgs,
  state,
  formData,
) {
  let invoice = formData.get("invoice");
  let transport = formData.get("transport");
  let message = validateData(invoice, transport, products, file);

  if (message.length > 6)
    return {
      error: true,
      completed: true,
      message: message,
    };

  const dbName = await getDbName();
  let chosenUser = formData.get("user");
  let org = formData.get("org");
  let date = formData.get("date").replaceAll("-", "_");
  let fileName = `../../database/${dbName}/documents/${invoice.replaceAll(/[\\./]/g, "").replaceAll(" ", "_")}_${chosenUser}${orgs.userOrgId}${org}_${date}${Date.now().toString()}.pdf`;
  let transformProducts = [];
  products.forEach((element) => {
    transformProducts.push({
      priceId: element.priceId,
      itemId: element.id,
      qty: element.qty,
      price: element.price,
      buyInvoiceId: element.invoiceId,
    });
  });

  let chosenCurrency = formData.get("currency");
  let currencyExchangeDate = getExchangeDate(formData);

  let invoiceData = getData();

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

  try {
    const userId = await getUserId();
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Invoices/add/sales/${userId}`,
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
        message: "Success! You had created sales invoice.",
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
    console.error(" fetch failed.");
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

  /**
   * Organize information into object for fetch.
   * @return {object}
   */
  function getData() {
    return {
      userId: parseInt(formData.get("user")),
      invoiceNumber: invoice,
      seller: orgs.userOrgId,
      buyer: parseInt(org),
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
      invoiceItems: transformProducts,
    };
  }
}
/**
 * If different date from invoice date was chosen for currency value then return chosen date, otherwise return invoice date.
 * @param  {FormData} formData Contain form data.
 * @return {string}      Date in string (yyyy-MM-dd).
 */
function getExchangeDate(formData) {
  return formData.get("currencyExchange")
    ? formData.get("currencyExchange")
    : formData.get("date");
}

/**
 * Validate form data.
 * @param  {string} invoice Invoice number.
 * @param  {string} transport String containing transport cost.
 * @param  {Array<object>} products Products added to proforma.
 * @param  {object} file File object.
 * @return {string}      Error message. If no error occurred only "Error:" is returned.
 */
function validateData(invoice, transport, products, file) {
  let message = "Error:";
  if (
    !validators.lengthSmallerThen(invoice, 40) ||
    !validators.stringIsNotEmpty(invoice)
  )
    message += "\nInvoice must not be empty.";
  if (
    !validators.isPriceFormat(transport) ||
    !validators.stringIsNotEmpty(transport)
  )
    message += "\nTransport cost must not be empty and must be decimal.";
  if (products.length <= 0) message += "\nInvoice must have products.";
  if (!file) message += "\nDocument must not be empty.";
  return message;
}
