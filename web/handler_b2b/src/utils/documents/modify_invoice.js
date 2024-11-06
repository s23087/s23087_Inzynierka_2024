"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";
import { getInvoicePath } from "./get_document_path";

const fs = require("node:fs");

/**
 * Sends request to modify invoice. When data is unchanged the attribute in request will be null.
 * @param  {FormData} file FormData object containing file binary data.
 * @param  {boolean} isYourInvoice Is invoice type "Yours invoices".
 * @param  {Number} invoiceId Invoice id.
 * @param  {{invoiceNumber: string, client: Number, transport: Number, paymentMethod: Number, paymentStatus: Number, status: boolean, note: string}} prevState Object that contain information about previous state of chosen invoice.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function updateInvoice(
  file,
  isYourInvoice,
  invoiceId,
  prevState,
  state,
  formData,
) {
  let invoiceNumber = formData.get("invoice");
  let transport = formData.get("transport");
  let client = parseInt(formData.get("org"));
  let paymentMethod = parseInt(formData.get("paymentMethod"));
  let paymentStatus = parseInt(formData.get("paymentStatus"));
  let status = formData.get("status") === "true";
  let path = null;
  let note = formData.get("note");
  let message = validateData(transport, invoiceNumber);

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }

  transport = parseFloat(transport);
  const dbName = await getDbName();
  let prevPath = await getInvoicePath(invoiceId);

  if (fileNeedToBeOverwritten(invoiceNumber, prevState, file)) {
    if (!prevPath) {
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
    const userId = await getUserId();
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Invoices/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 404) {
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
      if (data.invoiceNumber) {
        return await changePath(
          prevPath,
          data,
          dbName,
          userId,
          isYourInvoice,
          invoiceId,
        );
      }
      return {
        error: false,
        completed: true,
        message: "Success! You have modified the invoice.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error",
      };
    }
  } catch {
    console.error("updateInvoice fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }

  /**
   * Overwrite file with new data.
   * @return {Promise<string>} String containing path where file exist or will exist after renaming.
   */
  async function writeFile() {
    if (file) {
      let buffArray = await file.get("file").arrayBuffer();
      let buff = new Uint8Array(buffArray);
      fs.writeFileSync(prevPath, buff);
    }
    if (invoiceNumber !== prevState.invoiceNumber) {
      let newPath = prevPath.replace(
        prevState.invoiceNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_"),
        invoiceNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_"),
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
      isYourInvoice: isYourInvoice,
      invoiceId: invoiceId,
      invoiceNumber:
        invoiceNumber !== prevState.invoiceNumber ? invoiceNumber : null,
      clientId: client !== -1 ? client : null,
      transportCost: transport !== prevState.transport ? transport : null,
      paymentMethod: paymentMethod !== -1 ? paymentMethod : null,
      paymentStatus: paymentStatus !== -1 ? paymentStatus : null,
      inSystem: status !== prevState.status ? status : null,
      path: path ?? null,
      note: note !== prevState.note ? note : null,
    };
  }
}
/**
 * Checks if invoice file should be changed.
 * @param  {object} invoiceNumber Invoice number.
 * @param  {object} prevState Object that contain information about previous state of chosen invoice.
 * @param  {FormData} file FormData object containing file binary data.
 * @return {boolean}
 */
function fileNeedToBeOverwritten(invoiceNumber, prevState, file) {
  return invoiceNumber !== prevState.invoiceNumber || file;
}

/**
 * Sent request to change path in chosen invoice and rename file.
 * @param  {object} prevPath Previous path.
 * @param  {object} data Data from modify request.
 * @param  {string} dbName Database name.
 * @param  {Number} userId User id.
 * @param  {boolean} isYourInvoice Is invoice type "Yours invoices".
 * @param  {Number} invoiceId Invoice id.
 * @return {Promise<object>} Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
async function changePath(
  prevPath,
  data,
  dbName,
  userId,
  isYourInvoice,
  invoiceId,
) {
  try {
    fs.renameSync(prevPath, data.path);
  } catch (error) {
    const pathChange = await fetch(
      `${process.env.API_DEST}/${dbName}/Invoices/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify({
          isYourInvoice: isYourInvoice,
          invoiceId: invoiceId,
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
 * @param  {string} transport Transport cost.
 * @param  {string} invoiceNumber Invoice number.
 * @return {string} Return error message. If no error occurred return only "Error:"
 */
function validateData(transport, invoiceNumber) {
  let message = "Error:";
  if (
    !validators.isPriceFormat(transport) &&
    !validators.stringIsNotEmpty(transport)
  )
    message += "\nPrice must be a decimal and not empty.";
  if (
    !validators.lengthSmallerThen(invoiceNumber, 40) &&
    !validators.stringIsNotEmpty(invoiceNumber)
  )
    message += "\nInvoice number cannot be empty and exceed 40 chars.";
  return message;
}
