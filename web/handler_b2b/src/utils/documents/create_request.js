"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function createRequest(file, state, formData) {
  let recevier = parseInt(formData.get("user"));
  let type = formData.get("type");
  let note = formData.get("note");
  let title = formData.get("title");
  let message = "Error:";
  if (!recevier) message += "\nRecevier must not be empty.";
  if (!note) message += "\nNote must not be empty.";
  if (!title) message += "\nTitle must not be empty.";

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

  let requestData = {
    creatorId: userId,
    receiverId: recevier,
    objectType: type,
    path: file ? fileName : null,
    note: note,
    title,
    title,
  };

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

  const info = await fetch(`${process.env.API_DEST}/${dbName}/Requests/add`, {
    method: "POST",
    body: JSON.stringify(requestData),
    headers: {
      "Content-Type": "application/json",
    },
  });
  console.log(info.status);

  if (info.status === 404) {
    if (file) {
      try {
        fs.rmSync(fileName);
      } catch (error) {
        console.log(error);
      }
    }
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
    if (file) {
      try {
        fs.rmSync(fileName);
      } catch (error) {
        console.log(error);
      }
    }
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
      message: "Success! You had created request.",
    };
  } else {
    if (file) {
      try {
        fs.rmSync(fileName);
      } catch (error) {
        console.log(error);
        return {
          error: true,
          completed: true,
          message: "Critical file error.",
        };
      }
    }
    return {
      error: true,
      completed: true,
      message: "Critical error.",
    };
  }
}
