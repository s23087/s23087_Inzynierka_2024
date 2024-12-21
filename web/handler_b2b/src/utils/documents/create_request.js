"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

const fs = require("node:fs");

/**
 * Sends request to create sales invoice.
 * @param  {FormData} file FormData object containing file binary data.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function createRequest(file, state, formData) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let receiver = parseInt(formData.get("user"));
  let type = formData.get("type");
  let note = formData.get("note");
  let title = formData.get("title");
  let message = validateData(receiver, note, title);

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }

  const dbName = await getDbName();
  const userId = await getUserId();
  let fileName = `../../database/${dbName}/documents/req_${receiver}${userId}${type.replace(" ", "")}${Date.now().toString()}.pdf`;

  let requestData = getData();

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
    const info = await fetch(`${process.env.API_DEST}/${dbName}/Requests/add`, {
      method: "POST",
      body: JSON.stringify(requestData),
      headers: {
        "Content-Type": "application/json",
      },
    });

    let fetchError = await checkFetchErrors(info, file, fileName);
    if (fetchError) return fetchError;

    if (info.ok) {
      return {
        error: false,
        completed: true,
        message: "Success! You had created request.",
      };
    } else {
      if (file) {
        return removerFileWithFuncReturn(
          fileName,
          "Critical file error.",
          "Critical error.",
        );
      }
      return {
        error: true,
        completed: true,
        message: "Critical error.",
      };
    }
  } catch {
    console.error("Create request fetch failed.");
    return removerFileWithFuncReturn(
      fileName,
      "Connection error. Critical file error.",
      "Connection error.",
    );
  }
  /**
   * Organize information into object for fetch.
   * @return {object}
   */
  function getData() {
    return {
      creatorId: userId,
      receiverId: receiver,
      objectType: type,
      path: file ? fileName : null,
      note: note,
      title: title,
    };
  }
}

/**
 * Check if file exist and then delete.
 * @param  {Response} info Response of any fetch request.
 * @param  {string} file File data.
 * @param  {string} fileName Name of file to remove if error occurs.
 * @return {object} Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
async function checkFetchErrors(info, file, fileName) {
  if (info.status === 404) {
    deleteFile(file, fileName);
    let text = await info.text();
    if (text === "Creator not found.") {
      logout();
      return {
        error: true,
        completed: true,
        message: "Unauthorized.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: text,
      };
    }
  }

  if (info.status === 500) {
    deleteFile(file, fileName);
    return {
      error: true,
      completed: true,
      message: "Server error.",
    };
  }
}

/**
 * Delete file and return object with action result.
 * @param  {string} file File data.
 * @param  {string} errorText Error text of result.
 * @param  {string} successText Error text of success.
 * @return {object} Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
function removerFileWithFuncReturn(file, errorText, successText) {
  try {
    fs.rmSync(file);
    return {
      error: true,
      completed: true,
      message: successText,
    };
  } catch (error) {
    console.log(error);
    return {
      error: true,
      completed: true,
      message: errorText,
    };
  }
}

/**
 * Check if file exist and then delete.
 * @param  {string} file File data.
 * @param  {string} fileName Name of file to remove.
 * @return {string}      Error message. If no error occurred only "Error:" is returned.
 */
function deleteFile(file, fileName) {
  if (file) {
    try {
      fs.rmSync(fileName);
    } catch (error) {
      console.log(error);
    }
  }
}

/**
 * Validate form data.
 * @param  {string} receiver Receiver user id.
 * @param  {string} note User id.
 * @param  {string} title Request title.
 * @return {string}      Error message. If no error occurred only "Error:" is returned.
 */
function validateData(receiver, note, title) {
  let message = "Error:";
  if (!receiver) message += "\nReceiver must not be empty.";
  if (!note) message += "\nNote must not be empty.";
  if (!title) message += "\nTitle must not be empty.";
  return message;
}
