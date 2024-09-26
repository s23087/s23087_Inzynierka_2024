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
  let message = "Error:";
  let path = "";
  if (!recevierId) message += "\nRecevier must not be empty.";
  if (!note) message += "\nNote must not be empty.";

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
      if (file) {
        if (prevPath === "")
          prevPath = `../../database/${dbName}/documents/req_${recevierId}${userId}${objectType.replace(" ", "")}${Date.now().toString()}.pdf`;
        let buffArray = await file.get("file").arrayBuffer();
        let buff = new Uint8Array(buffArray);
        fs.writeFileSync(prevPath, buff);
      }
      if (objectType !== prevState.objectType) {
        let newPath = prevPath.replace(
          prevState.objectType.replaceAll(" ", ""),
          objectType.replaceAll(" ", ""),
        );
        path = newPath;
      }
    } catch (error) {
      return {
        error: true,
        completed: true,
        message: "Error: " + error,
      };
    }
  }

  let data = {
    requestId: requestId,
    recevierId: recevierId !== prevState.recevierId ? recevierId : null,
    objectType: objectType !== prevState.objectType ? objectType : null,
    path: path ? path : null,
    note: note !== prevState.note ? note : null,
  };

  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Requests/${userId}/modify`,
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
    if (data.objectType && prevPath !== path) {
      try {
        fs.renameSync(prevPath, data.path);
      } catch (error) {
        const pathChange = await fetch(
          `${process.env.API_DEST}/${dbName}/Requests/{userId}/modify?userId=${userId}`,
          {
            method: "POST",
            body: JSON.stringify({
              requestId: invoiceId,
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
}
