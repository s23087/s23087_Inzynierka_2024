"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import getUserOrgName from "./get_org_name";

const fs = require("node:fs");

/**
 * Sends request to modify pricelist. When data is unchanged the attribute in request will be null. If user do not exist server will logout them.
 * @param  {Array<{id: Number, price: Number}>} products Array of products.
 * @param  {string} currency Currency shortcut name.
 * @param  {Number} offerId Offer id.
 * @param  {{offerName: string, maxQty: Number, currency: string, type: string, path: string,}} prevState Object that contain information about previous state of chosen item.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function modifyPricelist(
  products,
  currency,
  offerId,
  prevState,
  state,
  formData,
) {
  let offerName = formData.get("name");
  let status = formData.get("status");
  let type = formData.get("type");
  let maxQtyForm = formData.get("maxQty");
  let errorMessage = validateData(status, offerName, maxQtyForm, products);

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  const userId = await getUserId();
  const dbName = await getDbName();
  const deactivatedId = await getDeactivatedId(dbName);
  if (!deactivatedId)
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  let orgName = await getUserOrgName();
  if (!orgName) {
    return {
      error: true,
      completed: true,
      message: "Could not download necessary info.",
    };
  }
  orgName = orgName.replace(/[^a-zA-Z0-9]/, "");
  orgName = orgName.replace(" ", "");
  let fileName = "";
  let attributeChanged = getAttributeChanged(
    offerName,
    prevState,
    maxQtyForm,
    currency,
    type,
  );

  if (requireNewPath(attributeChanged, status, deactivatedId)) {
    fileName = `src/app/api/pricelist/${orgName}/${offerName.replaceAll(" ", "_").replaceAll(/\W/g, 25 + userId)}${maxQtyForm}${currency}${Date.now().toString()}.${type}`;
    try {
      let result = renameFile(fileName, prevState);
      if (result) {
        return result;
      }
    } catch (error) {
      console.log(error);
      return {
        error: true,
        completed: true,
        message: "Server file error.",
      };
    }
  }
  if (status === deactivatedId) {
    try {
      fs.rmSync(prevState.path);
    } catch (error) {
      console.log(error);
      return {
        error: true,
        completed: true,
        message: "Could not delete old file.",
      };
    }
  }

  let transformProducts = [];
  products.forEach((element) => {
    transformProducts.push({
      itemId: element.id,
      price: element.price,
    });
  });

  let data = getData();

  try {
    const info = await fetch(`${process.env.API_DEST}/${dbName}/Offer/modify`, {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    });

    let fetchError = await checkFetchForError(info);
    if (fetchError) {
      return fetchError;
    }

    if (info.ok) {
      return {
        error: false,
        completed: true,
        message: "Success! You had modified pricelist.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error.",
      };
    }
  } catch {
    console.error("Modify offer fetch failed.");
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
      userId: userId,
      offerId: offerId,
      offerName: offerName !== prevState.offerName ? offerName : null,
      offerStatusId: parseInt(status) !== -1 ? parseInt(status) : null,
      maxQty:
        parseInt(maxQtyForm) !== prevState.maxQty ? parseInt(maxQtyForm) : null,
      currencyName: currency !== prevState.currency ? currency : null,
      path: fileName === "" ? null : fileName,
      type: type !== prevState.type ? type : null,
      items: transformProducts,
    };
  }
}
/**
 * Rename file. If error occur return object, otherwise return null.
 * @param {string} fileName New file name.
 * @param  {object} prevState Previous state of pricelist.
 * @return {object}      Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
function renameFile(fileName, prevState) {
  if (fs.existsSync(fileName)) {
    return {
      error: true,
      completed: true,
      message: "That pricelist already exist.",
    };
  } else if (fs.existsSync(prevState.path)) {
    fs.renameSync(prevState.path, fileName);
    return null;
  } else {
    return {
      error: true,
      completed: true,
      message: "Original file does not exists.",
    };
  }
}
/**
 * Check response for error. If no error occurred return null. If error 404 with text "User not found." is received, the user will be logout.
 * @param {Response} info Fetch response.
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
      message: text,
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
 * Fetch for "Deactivated" status id. If connection lost return null.
 * @param {string} dbName Name of database.
 * @return {Promise<string>}      Return id of status "Deactivated".
 */
async function getDeactivatedId(dbName) {
  try {
    let getDeactivatedIdFetch = await fetch(
      `${process.env.API_DEST}/${dbName}/Offer/get/status/deactivated`,
      { method: "GET" },
    );
    return getDeactivatedIdFetch.text();
  } catch {
    console.error("Get status deactivated fetch failed.");
    return null;
  }
}
/**
 * Checks if pricelist path should be changed.
 * @param  {object} attributeChanged Boolean that tells if at least one attribute changed.
 * @param  {object} status Chosen status id.
 * @param  {string} deactivatedId Status id with name "Deactivated".
 * @return {boolean} True if attribute changes and status is still active, otherwise false.
 */
function requireNewPath(attributeChanged, status, deactivatedId) {
  return attributeChanged && status !== deactivatedId;
}

/**
 * Check if at least one of given attribute has changed.
 * @param  {string} offerName Offer name.
 * @param  {object} prevState Object that contain information about previous state of chosen item.
 * @param  {string} maxQtyForm Max showed number of product in pricelist.
 * @param  {string} currency Currency shortcut name.
 * @param  {string} type Pricelist type.
 * @return {boolean} True if at least one changed, otherwise false.
 */
function getAttributeChanged(offerName, prevState, maxQtyForm, currency, type) {
  return (
    offerName !== prevState.offerName ||
    maxQtyForm !== prevState.maxQty ||
    currency !== prevState.currency ||
    type !== prevState.type
  );
}

/**
 * Validate given form data.
 * @param  {string} status Status id.
 * @param  {string} offerName Offer name.
 * @param  {string} maxQtyForm Max showed number of product in pricelist.
 * @param  {Array<object>} products Array containing products.
 * @return {string} Return error message. If no error occurred return only "Error:"
 */
function validateData(status, offerName, maxQtyForm, products) {
  let errorMessage = "Error:";
  if (!status) errorMessage += "\nStatus must not be empty.";
  if (
    !validators.stringIsNotEmpty(offerName) ||
    !validators.lengthSmallerThen(offerName, 100)
  ) {
    errorMessage += "\nOffer name must not be empty or exceed 100 chars.";
  }
  if (
    !validators.stringIsNotEmpty(maxQtyForm) ||
    !validators.haveOnlyNumbers(maxQtyForm)
  ) {
    errorMessage += "\nMax qty must be a number and not empty.";
  }
  if (products.length === 0) errorMessage += "\nProducts must note be empty.";
  return errorMessage;
}
