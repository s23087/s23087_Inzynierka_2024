"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import getUserOrgName from "./get_org_name";

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
  const deactivatedId = await getDeactivatedId();
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
  let attributeChanged = getAtributeChanged(
    offerName,
    prevState,
    maxQtyForm,
    currency,
    type,
  );
  const fs = require("node:fs");
  if (requireNewPath(attributeChanged, status, deactivatedId)) {
    fileName = `src/app/api/pricelist/${orgName}/${offerName.replaceAll(" ", "_").replaceAll(/\W/g, 25 + userId)}${maxQtyForm}${currency}${Date.now().toString()}.${type}`;
    try {
      let result = renameFile(fileName, prevState, fs)
      if (result) {
        return result
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

    let fetchError = await checkFetchForError(info)
    if (fetchError) {
      return fetchError
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
function renameFile(fileName, prevState, fs){
  if (fs.existsSync(fileName)) {
    return {
      error: true,
      completed: true,
      message: "That pricelist already exist.",
    };
  } else if (fs.existsSync(prevState.path)) {
    fs.renameSync(prevState.path, fileName);
    return null
  } else {
    return {
      error: true,
      completed: true,
      message: "Original file does not exists.",
    };
  }
}

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
  return null
}

async function getDeactivatedId() {
  try {
    let getDeactivatedId = await fetch(
      `${process.env.API_DEST}/${dbName}/Offer/get/status/deactivated`,
      { method: "GET" },
    );
    return getDeactivatedId.text();
  } catch {
    console.error("Get status deactivated fetch failed.");
    return null;
  }
}

function requireNewPath(attributeChanged, status, deactivatedId) {
  return attributeChanged && status !== deactivatedId;
}

function getAtributeChanged(offerName, prevState, maxQtyForm, currency, type) {
  return (
    offerName !== prevState.offerName ||
    maxQtyForm !== prevState.maxQty ||
    currency !== prevState.currency ||
    type !== prevState.type
  );
}

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
