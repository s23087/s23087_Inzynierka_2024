"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import { getRequestPath } from "./get_document_path";

export default async function updateRequest(
  file,
  prevState,
  requestId,
  state,
  formData,
) {
  const fs = require("node:fs");
  let recevierId = parseInt(formData.get("user"));
  let objectType = formData.get("type");
  let note = formData.get("note");
  let title = formData.get("title");
  let path = "";
  let message = validateData(recevierId, note, title);

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
      if (shouldpathNameChange(data, prevPath, path)) {
        return await changePath(prevPath, data, dbName, userId, requestId, fs);
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

  async function writeFile() {
    if (prevPath === "")
      prevPath = `../../database/${dbName}/documents/req_${recevierId}${userId}${objectType.replace(" ", "")}${Date.now().toString()}.pdf`;
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

  function getData() {
    return {
      requestId: requestId,
      recevierId: recevierId !== prevState.recevierId ? recevierId : null,
      objectType: objectType !== prevState.objectType ? objectType : null,
      path: path ?? null,
      note: note !== prevState.note ? note : null,
      title: title !== prevState.title ? title : null,
    };
  }
}

async function changePath(prevPath, data, dbName, userId, requestId, fs) {
  try {
    fs.renameSync(prevPath, data.path);
  } catch (error) {
    const pathChange = await fetch(
      `${process.env.API_DEST}/${dbName}/Requests/{userId}/modify/${userId}`,
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

function shouldpathNameChange(data, prevPath, path) {
  return data.objectType && prevPath !== path;
}

function validateData(recevierId, note, title) {
  let message = "Error:";
  if (!recevierId) message += "\nRecevier must not be empty.";
  if (!note) message += "\nNote must not be empty.";
  if (!title) message += "\nTitle must not be empty.";
  return message;
}
