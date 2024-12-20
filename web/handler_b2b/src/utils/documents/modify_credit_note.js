"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import { getCreditPath } from "./get_document_path";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to modify credit note. When data is unchanged the attribute in request will be null.
 * @param  {FormData} file FormData object containing file binary data.
 * @param  {boolean} isYourCredit Is credit note type "Yours credit notes".
 * @param  {Number} creditNoteId Credit note id.
 * @param  {{creditNoteNumber: string, date: string, inSystem: boolean, isPaid: boolean, note: string}} prevState Object that contain information about previous state of chosen credit note.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function updateCreditNote(
  file,
  isYourCredit,
  creditNoteId,
  prevState,
  state,
  formData,
) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const fs = require("node:fs");
  let creditNoteNumber = formData.get("creditNumber");
  let date = formData.get("date");
  let status = formData.get("status") === "true";
  let isPaid = formData.get("isPaid") === "on";
  let path = null;
  let note = formData.get("note");
  let message = validateData(creditNoteNumber);

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }

  const dbName = await getDbName();
  let prevPath = await getCreditPath(creditNoteId);

  if (fileNeedToBeOverwritten(creditNoteNumber, prevState, file)) {
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
      `${process.env.API_DEST}/${dbName}/CreditNote/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 404) {
      let infoText = await info.text();
      if (infoText.includes("User")) logout();
      return {
        error: true,
        completed: true,
        message: infoText,
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
      if (data.creditNumber) {
        return await changeObjectPath(
          prevPath,
          data,
          dbName,
          userId,
          isYourCredit,
          creditNoteId,
        );
      }
      return {
        error: false,
        completed: true,
        message: "Success! You have modified the credit note.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error",
      };
    }
  } catch {
    console.error("updateCreditNote fetch failed.");
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
    if (creditNoteNumber !== prevState.invoiceNumber) {
      let newPath = prevPath.replace(
        prevState.creditNoteNumber
          .replaceAll(/[\\./]/g, "")
          .replaceAll(" ", "_"),
        creditNoteNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_"),
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
      isYourCredit: isYourCredit,
      id: creditNoteId,
      creditNumber:
        creditNoteNumber !== prevState.creditNoteNumber
          ? creditNoteNumber
          : null,
      date: date !== prevState.date ? date : null,
      inSystem: status !== prevState.inSystem ? status : null,
      isPaid: isPaid !== prevState.isPaid ? isPaid : null,
      path: path ?? null,
      note: note !== prevState.note ? note : null,
    };
  }
}
/**
 * Checks if credit note file should be changed.
 * @param  {object} creditNoteNumber Credit note number.
 * @param  {object} prevState Object that contain information about previous state of chosen credit note.
 * @param  {FormData} file FormData object containing file binary data.
 * @return {boolean}
 */
function fileNeedToBeOverwritten(creditNoteNumber, prevState, file) {
  return creditNoteNumber !== prevState.creditNoteNumber || file;
}

/**
 * Sent request to change path in chosen credit note and rename file.
 * @param  {object} prevPath Previous path.
 * @param  {object} data Data from modify request.
 * @param  {string} dbName Database name.
 * @param  {Number} userId User id.
 * @param  {boolean} isYourCredit Is credit note type "Yours credit notes".
 * @param  {Number} creditNoteId Credit note id.
 * @return {Promise<object>} Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
async function changeObjectPath(
  prevPath,
  data,
  dbName,
  userId,
  isYourCredit,
  creditNoteId,
) {
  const fs = require("node:fs");
  try {
    fs.renameSync(prevPath, data.path);
  } catch (error) {
    const pathChange = await fetch(
      `${process.env.API_DEST}/${dbName}/CreditNote/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify({
          isYourCredit: isYourCredit,
          id: creditNoteId,
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
 * @param  {string} creditNoteNumber Credit note number.
 * @return {string} Return error message. If no error occurred return only "Error:"
 */
function validateData(creditNoteNumber) {
  let message = "Error:";
  if (
    !validators.lengthSmallerThen(creditNoteNumber, 40) &&
    !validators.stringIsNotEmpty(creditNoteNumber)
  )
    message += "\nInvoice number cannot be empty and exceed 40 chars.";
  return message;
}
