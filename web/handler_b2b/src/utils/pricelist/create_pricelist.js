"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import getUserOrgName from "./get_org_name";

/**
 * Sends request to create pricelist. If user is not found in database server will logout them.
 * @param  {Array<{id: Number, price: Number, purchasePrice: Number}>} products Array of products.
 * @param  {string} currency Currency shortcut name.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function createPricelist(
  products,
  currency,
  state,
  formData,
) {
  let offerName = formData.get("name");
  let status = formData.get("status");
  let maxQty = formData.get("maxQty");
  let type = formData.get("type");
  let errorMessage = validateData(status, offerName, maxQty, products);

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  const userId = await getUserId();
  const dbName = await getDbName();
  let orgName = await getUserOrgName();
  if (orgName === "404") {
    logout();
    return {
      error: true,
      completed: true,
      message: "Your account does not exist.",
    };
  }
  if (!orgName) {
    return {
      error: true,
      completed: true,
      message: "Could not connect to server.",
    };
  }
  orgName = orgName.replace(/[^a-zA-Z0-9]/, "");
  orgName = orgName.replace(" ", "");
  let fileName = `src/app/api/pricelist/${orgName}/${offerName.replaceAll(" ", "_").replaceAll(/\W/g, 25 + userId)}${maxQty}${currency}${Date.now().toString()}.${type}`;

  const fs = require("node:fs");
  try {
    if (fs.existsSync(fileName)) {
      return {
        error: true,
        completed: true,
        message: "That pricelist already exist.",
      };
    }
  } catch (error) {
    console.log(error);
    return {
      error: true,
      completed: true,
      message: "Server file error.",
    };
  }

  let transformProducts = [];
  products.forEach((element) => {
    transformProducts.push({
      itemId: element.id,
      price: element.price,
    });
  });

  let data = {
    userId: userId,
    offerName: offerName,
    path: fileName,
    offerStatusId: parseInt(status),
    maxQty: parseInt(maxQty),
    currency: currency,
    items: transformProducts,
  };
  try {
    const info = await fetch(`${process.env.API_DEST}/${dbName}/Offer/add`, {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    });
    if (info.status === 404) {
      logout();
      return {
        error: true,
        completed: true,
        message: "Your user profile does not exists.",
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
        message: "Success! You had created pricelist.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error.",
      };
    }
  } catch {
    console.error("createPricelist fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
/**
 * Validate given form data.
 * @param  {string} status Status id.
 * @param  {string} offerName Offer name.
 * @param  {string} maxQtyForm Max showed number of product in pricelist.
 * @param  {Array<object>} products Transport cost in form of string.
 * @return {string} Return error message. If no error occurred retrun only "Error:"
 */
function validateData(status, offerName, maxQty, products) {
  let errorMessage = "Error:";
  if (!status) errorMessage += "\nStatus must not be empty.";
  if (
    !validators.stringIsNotEmpty(offerName) ||
    !validators.lengthSmallerThen(offerName, 100)
  ) {
    errorMessage += "\nOffer name must not be empty or exceed 100 chars.";
  }
  if (
    !validators.stringIsNotEmpty(maxQty) ||
    !validators.haveOnlyNumbers(maxQty, 100)
  ) {
    errorMessage += "\nMax qty must be a number and not empty.";
  }
  if (products.length === 0) errorMessage += "\nProducts must note be empty.";
  return errorMessage;
}
