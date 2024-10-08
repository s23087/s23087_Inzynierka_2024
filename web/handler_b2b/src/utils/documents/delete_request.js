"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import { getRequestPath } from "./get_document_path";

export default async function deleteRequest(requestId) {
  const dbName = await getDbName();
  const path = await getRequestPath(requestId);
  if (path === null) {
    return {
      error: true,
      message: "Request do not exists.",
    };
  }
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/Requests/delete/${requestId}?userId=${userId}`;
  const info = await fetch(url, {
    method: "Delete",
  });

  if (info.ok) {
    if (path === "") {
      return {
        error: false,
        message: "Success!",
      };
    }
    const fs = require("node:fs");
    try {
      fs.rmSync(path);
      return {
        error: false,
        message: "Success!",
      };
    } catch (error) {
      console.log(error);
      return {
        error: true,
        message: "Success with error. Could not delete file on server.",
      };
    }
  }
  return {
    error: true,
    message: await info.text(),
  };
}