"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import getCurrencyValueByDate from "../flexible/get_chosen_currency_value";
import validators from "../validators/validator";

/**
 * Sends request to create proforma.
 * @param  {Array<{id: Number, priceId: Number|undefined, qty: Number, price: Number}>} products Products added to proforma. PriceId is undefined only if isYourProforma is true.
 * @param  {FormData} file FormData object containing file binary data.
 * @param  {{orgName: string, restOrgs: Array<{orgName: string, orgId: Number}>}} orgs Object that contain user organization name.
 * @param  {boolean} isYourProforma Is proforma type "Yours proformas".
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function createProforma(
  products,
  file,
  orgs,
  isYourProforma,
  state,
  formData,
) {
  let chosenUser = formData.get("user");
  let proformaNumber = formData.get("proformaNumber");
  let seller = formData.get("org");
  let taxes = formData.get("taxes");
  let chosenCurrency = formData.get("currency");
  let transport = formData.get("transport");
  let paymentMethod = formData.get("paymentMethod");
  let errorMessage = validateData(formData, products);

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  const userId = await getUserId();
  const dbName = await getDbName();
  let proformaDate = formData.get("date").replaceAll("-", "_");
  let fileName = `../../database/${dbName}/documents/pr_${proformaNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_")}_${chosenUser}${orgs.userOrgId}${seller}_${proformaDate}${userId}${Date.now().toString()}.pdf`;
  let transformProducts = getTransformedProducts(products, isYourProforma);
  let currencyExchangeDate = getCurrencyExchangeDate(formData);
  let curVal = await getCurVal(currencyExchangeDate, chosenCurrency);

  if (curVal === 0) {
    return {
      error: true,
      completed: true,
      message: "Could not download currency value.",
    };
  }

  let proformaData = getData();

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
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Proformas/add/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(proformaData),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    let fetchError = await checkFetchForError(info);
    if (fetchError) {
      return fetchError;
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
  } catch {
    console.error("createProforma fetch failed.");
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
      isYourProforma: isYourProforma,
      proformaNumber: proformaNumber,
      sellerId: isYourProforma ? parseInt(seller) : orgs.userOrgId,
      buyerId: isYourProforma ? orgs.userOrgId : parseInt(seller),
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
  }
}
/**
 * Check response for error statuses. If no error occurred return null.
 * @param  {Response} info Fetch response.
 * @return {Promise<object>}      Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
async function checkFetchForError(info) {
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
  return null;
}

/**
 * Check response for error statuses. If no error occurred return null.
 * @param  {Array<object>} products Products added to proforma.
 * @param  {boolean} isYourProforma Is proforma type "Yours proformas".
 * @return {Array<object>}      Return products prepared for fetch data.
 */
function getTransformedProducts(products, isYourProforma) {
  let transformProducts = [];
  products.forEach((element) => {
    transformProducts.push({
      itemId: isYourProforma ? element.id : element.priceId,
      qty: element.qty,
      price: element.price,
    });
  });
  return transformProducts;
}

/**
 * If different date from proforma date was chosen for currency value then return chosen date, otherwise return proforma date.
 * @param  {FormData} formData Contain form data.
 * @return {string}      Date in string (yyyy-MM-dd).
 */
function getCurrencyExchangeDate(formData) {
  return formData.get("currencyExchange")
    ? formData.get("currencyExchange")
    : formData.get("date");
}

/**
 * Return value of chosen currency in given date from NBP site. If date is sunday or saturday then it will take date from last friday.
 * @param  {string} currencyExchangeDate Date in string (yyyy-MM-dd).
 * @param  {string} chosenCurrency Shortcut for currency
 * @return {Promise<Number>}      Currency value from NBP site.
 */
async function getCurVal(currencyExchangeDate, chosenCurrency) {
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
  return curVal;
}

/**
 * Validate form data.
 * @param  {FormData} formData Contain form data.
 * @param  {Array<object>} products Products added to proforma.
 * @return {string}      Error message. If no error occurred only "Error:" is returned.
 */
function validateData(formData, products) {
  let errorMessage = "Error:";
  if (!formData.get("paymentMethod"))
    errorMessage += "\nPayment method must not be empty.";
  if (!formData.get("currency"))
    errorMessage += "\nCurrency must not be empty.";
  if (!formData.get("taxes")) errorMessage += "\nTaxes must not be empty.";
  if (!formData.get("org")) errorMessage += "\nClient must not be empty.";
  if (!formData.get("user")) errorMessage += "\nUser must not be empty.";
  if (
    !validators.lengthSmallerThen(formData.get("proformaNumber"), 40) ||
    !validators.stringIsNotEmpty(formData.get("proformaNumber"))
  )
    errorMessage += "\nProforma number must not be empty.";
  if (
    !validators.isPriceFormat(formData.get("transport")) ||
    !validators.stringIsNotEmpty(formData.get("transport"))
  )
    errorMessage += "\nTransport cost must not be empty and must be decimal.";
  if (products.length <= 0) errorMessage += "\nProforma must have products.";
  return errorMessage;
}
