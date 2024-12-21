"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import getProformaPath from "./get_proforma_path";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

const fs = require("node:fs");

/**
 * Sends request to modify proforma. When data is unchanged the attribute in request will be null.
 * @param  {FormData} file FormData object containing file binary data.
 * @param  {{orgName: string, restOrgs: Array<{orgName: string, orgId: Number}>}} orgs Object that contain user organization name.
 * @param  {{proformaNumber: string, userId: Number, client: Number, transport: Number, paymentMethod: Number, status: boolean, note: string}} prevState Object that contain information about previous state of chosen item.
 * @param  {Number} proformaId Proforma id.
 * @param  {boolean} isYourProforma Is proforma type "Yours proformas".
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function updateProforma(
  file,
  orgs,
  prevState,
  proformaId,
  isYourProforma,
  state,
  formData,
) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let user = formData.get("user");
  let proformaNumber = formData.get("proformaNumber");
  let note = formData.get("note");
  let org = formData.get("org");
  let transport = formData.get("transport");
  let paymentMethod = formData.get("paymentMethod");
  let status = formData.get("status") === "true";
  let path = "";
  let message = validateData(proformaNumber, user, org, transport);

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }
  const dbName = await getDbName();
  const userId = await getUserId();
  let prevPath = await getProformaPath(proformaId);

  if (file) {
    if (prevPath === null) {
      return {
        error: true,
        completed: true,
        message: "Error: Could not download file path from server.",
      };
    }
    try {
      path = await writeFile();
    } catch (error) {
      return {
        error: true,
        completed: true,
        message: "Error: " + error,
      };
    }
  }

  let data = getData();

  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Proformas/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 404) {
      let text = await info.text();
      if (text === "User not found.") logout();
      return {
        error: true,
        completed: true,
        message: text,
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
      if (needNewPath(data, prevPath, path)) {
        return await changePath(
          prevPath,
          data,
          dbName,
          userId,
          isYourProforma,
          proformaId,
        );
      }
      return {
        error: false,
        completed: true,
        message: "Success! You have modified the request.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error",
      };
    }
  } catch {
    console.error("updateProforma fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error",
    };
  }
  /**
   * Overwrite file with new data.
   * @return {Promise<string>} String containing path where file exist or will exist after renaming.
   */
  async function writeFile() {
    if (prevPath === "")
      prevPath = `../../database/${dbName}/documents/pr_${proformaNumber.replaceAll("/", "")}_${user}${orgs.userOrgId}${org}_${userId}${Date.now().toString()}.pdf`;
    let buffArray = await file.get("file").arrayBuffer();
    let buff = new Uint8Array(buffArray);
    fs.writeFileSync(prevPath, buff);
    if (proformaNumber !== prevState.proformaNumber) {
      let newPath = prevPath.replace(
        prevState.proformaNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_"),
        proformaNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_"),
      );
      return newPath;
    }
    return path;
  }

  /**
   * Organize information into object for fetch.
   * @return {object}
   */
  function getData() {
    return {
      isYourProforma: isYourProforma,
      proformaId: proformaId,
      userId: parseInt(user) !== prevState.userId ? parseInt(user) : null,
      proformaNumber:
        proformaNumber !== prevState.proformaNumber ? proformaNumber : null,
      clientId: parseInt(org) !== -1 ? parseInt(org) : null,
      transport:
        parseFloat(transport) !== prevState.transport
          ? parseFloat(transport)
          : null,
      paymentMethodId:
        parseInt(paymentMethod) !== -1 ? parseInt(paymentMethod) : null,
      inSystem: status !== prevState.status ? status : null,
      path: path ?? null,
      note: note !== prevState.note ? note : null,
    };
  }
}
/**
 * Checks if proforma path should be changed.
 * @param  {object} data Data from modify request.
 * @param  {object} prevPath Previous path.
 * @param  {string} path Modified path name.
 * @return {boolean}
 */
function needNewPath(data, prevPath, path) {
  return data.proformaNumber && prevPath !== path;
}

/**
 * Sent request to change path in chosen proforma and rename file.
 * @param  {object} prevPath Previous path.
 * @param  {object} data Data from modify request.
 * @param  {string} dbName Database name.
 * @param  {Number} userId User id.
 * @param  {boolean} isYourProforma Is proforma type "Yours proformas".
 * @param  {Number} proformaId Proforma id.
 * @return {Promise<object>} Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
async function changePath(
  prevPath,
  data,
  dbName,
  userId,
  isYourProforma,
  proformaId,
) {
  try {
    fs.renameSync(prevPath, data.path);
  } catch (error) {
    const pathChange = await fetch(
      `${process.env.API_DEST}/${dbName}/Proformas/${userId}/modify`,
      {
        method: "POST",
        body: JSON.stringify({
          isYourProforma: isYourProforma,
          proformaId: proformaId,
          path: prevPath,
        }),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );
    if (pathChange.ok) {
      return {
        error: true,
        completed: true,
        message: "Success with errors! The file has not been renamed.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error. Invoice has been updated but file not.",
      };
    }
  }
}

/**
 * Validate given form data.
 * @param  {string} proformaNumber Proforma number.
 * @param  {string} user User id.
 * @param  {string} org Organization id.
 * @param  {string} transport Transport cost in form of string.
 * @return {string} Return error message. If no error occurred return only "Error:"
 */
function validateData(proformaNumber, user, org, transport) {
  let message = "Error:";
  if (!proformaNumber || proformaNumber.length > 40)
    message += "\nProforma must not be empty or exceed 40 chars.";
  if (!user) message += "\nReceiver must not be empty.";
  if (!org) message += "\nClient must not be empty.";
  if (!transport || !validators.isPriceFormat(transport))
    message += "\nTransport must be a number and not empty.";
  return message;
}
