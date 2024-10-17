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
  let errorMessage = "Error:";
  let offerName = formData.get("name");
  let status = formData.get("status");
  let type = formData.get("type");
  let maxQtyForm = formData.get("maxQty");
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

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  const userId = await getUserId();
  const dbName = await getDbName();
  let getDeactivatedId;
  try {
    getDeactivatedId = await fetch(
      `${process.env.API_DEST}/${dbName}/Offer/get/status/deactivated`,
      { method: "GET" },
    );
  } catch {
    console.error("Get status deactivated fetch failed.")
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
  const deactivatedId = await getDeactivatedId.text();
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
  let fileName = "";
  let attributeChanged =
    offerName !== prevState.offerName ||
    maxQtyForm !== prevState.maxQty ||
    currency !== prevState.currency ||
    type !== prevState.type;
  const fs = require("node:fs");
  if (attributeChanged && status !== deactivatedId) {
    fileName = `src/app/api/pricelist/${orgName}/${offerName.replaceAll(" ", "_").replaceAll(/[^a-zA-Z0-9_]/g, 25 + userId)}${maxQtyForm}${currency}${Date.now().toString()}.${type}`;
    try {
      if (fs.existsSync(fileName)) {
        return {
          error: true,
          completed: true,
          message: "That pricelist already exist.",
        };
      } else {
        if (fs.existsSync(prevState.path))
          fs.renameSync(prevState.path, fileName);
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

  let data = {
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

  try {
    const info = await fetch(`${process.env.API_DEST}/${dbName}/Offer/modify`, {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    });
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
    console.error("Modify offer fetch failed.")
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
