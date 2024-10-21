"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function createRequest(file, state, formData) {
  let recevier = parseInt(formData.get("user"));
  let type = formData.get("type");
  let note = formData.get("note");
  let title = formData.get("title");
  let message = validateData(recevier, note, title);

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }

  const dbName = await getDbName();
  const userId = await getUserId();
  let fileName = `../../database/${dbName}/documents/req_${recevier}${userId}${type.replace(" ", "")}${Date.now().toString()}.pdf`;

  let requestData = getData();

  const fs = require("node:fs");
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

    let fetchError = await checkFetchErrors(info, file, fs, fileName);
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
          fs,
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
      fs,
    );
  }

  function getData() {
    return {
      creatorId: userId,
      receiverId: recevier,
      objectType: type,
      path: file ? fileName : null,
      note: note,
      title: title,
    };
  }
}

async function checkFetchErrors(info, file, fs, fileName) {
  if (info.status === 404) {
    deleteFile(file, fs, fileName);
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
    deleteFile(file, fs, fileName);
    return {
      error: true,
      completed: true,
      message: "Server error.",
    };
  }
}

function removerFileWithFuncReturn(file, errorText, successText, fs) {
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

function deleteFile(file, fs, fileName) {
  if (file) {
    try {
      fs.rmSync(fileName);
    } catch (error) {
      console.log(error);
    }
  }
}

function validateData(recevier, note, title) {
  let message = "Error:";
  if (!recevier) message += "\nRecevier must not be empty.";
  if (!note) message += "\nNote must not be empty.";
  if (!title) message += "\nTitle must not be empty.";
  return message;
}
