"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import { getRequestPath } from "./get_document_path";

const fs = require("node:fs");
/**
 * Sends request to modify request. When data is unchanged the attribute in request will be null. If user do not exist server will logout them.
 * @param  {FormData} file FormData with a file data in binary.
 * @param  {{receiverId: Number, objectType: string, note: string, title: string}} prevState Object that contain information about previous state of chosen item.
 * @param  {Number} requestId Request id.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function updateRequest(
  file,
  prevState,
  requestId,
  state,
  formData,
) {
  let receiverId = parseInt(formData.get("user"));
  let objectType = formData.get("type");
  let note = formData.get("note");
  let title = formData.get("title");
  let path = "";
  let message = validateData(receiverId, note, title);

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }
  const dbName = await getDbName();
  const userId = await getUserId();
  let prevPath = await getRequestPath(requestId);

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
      `${process.env.API_DEST}/${dbName}/Requests/modify/${userId}`,
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
      if (shouldPathNameChange(data, prevPath, path)) {
        return await changePath(prevPath, data, dbName, userId, requestId);
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
    console.error("updateRequest fetch failed.");
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
    if (prevPath === "")
      prevPath = `../../database/${dbName}/documents/req_${receiverId}${userId}${objectType.replace(" ", "")}${Date.now().toString()}.pdf`;
    let buffArray = await file.get("file").arrayBuffer();
    let buff = new Uint8Array(buffArray);
    fs.writeFileSync(prevPath, buff);
    if (objectType !== prevState.objectType) {
      let newPath = prevPath.replace(
        prevState.objectType.replaceAll(" ", ""),
        objectType.replaceAll(" ", ""),
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
      requestId: requestId,
      receiverId: receiverId !== prevState.receiverId ? receiverId : null,
      objectType: objectType !== prevState.objectType ? objectType : null,
      path: path ?? null,
      note: note !== prevState.note ? note : null,
      title: title !== prevState.title ? title : null,
    };
  }
}
/**
 * Sent request to change path in chosen request and rename file.
 * @param  {object} prevPath Previous path.
 * @param  {object} data Data from modify request.
 * @param  {string} dbName Database name.
 * @param  {Number} userId User id.
 * @param  {Number} requestId Request id.
 * @return {Promise<object>} Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
async function changePath(prevPath, data, dbName, userId, requestId) {
  try {
    fs.renameSync(prevPath, data.path);
  } catch (error) {
    const pathChange = await fetch(
      `${process.env.API_DEST}/${dbName}/Requests/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify({
          requestId: requestId,
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
 * Checks if request path should be changed.
 * @param  {object} data Data from modify request.
 * @param  {object} prevPath Previous path.
 * @param  {string} path Modified path name.
 * @return {boolean}
 */
function shouldPathNameChange(data, prevPath, path) {
  return data.objectType && prevPath !== path;
}
/**
 * Validate given form data.
 * @param  {Number} receiverId Receiver id.
 * @param  {string} note User note.
 * @param  {string} title Request title.
 * @return {string} Return error message. If no error occurred return only "Error:"
 */
function validateData(receiverId, note, title) {
  let message = "Error:";
  if (!receiverId) message += "\nReceiver must not be empty.";
  if (!note) message += "\nNote must not be empty.";
  if (!title) message += "\nTitle must not be empty.";
  return message;
}
