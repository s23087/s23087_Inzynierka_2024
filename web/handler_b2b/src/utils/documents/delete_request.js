"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import { getRequestPath } from "./get_document_path";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to delete chosen user request. If user do not exist server will logout them.
 * @param  {Number} requestId Request id.
 * @return {Promise<{error: boolean, message: string}>} Return action result with message. If error is true that action was unsuccessful.
 */
export default async function deleteRequest(requestId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const path = await getRequestPath(requestId);
  if (path === null) {
    return {
      error: true,
      message: "Server error.",
    };
  }
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/Requests/delete/${requestId}/user/${userId}`;
  try {
    const info = await fetch(url, {
      method: "Delete",
    });

    if (info.ok) {
      if (path === "") {
        return {
          error: false,
          message: "Success! Deleted request.",
        };
      }
      const fs = require("node:fs");
      try {
        fs.rmSync(path);
        return {
          error: false,
          message: "Success! Deleted request.",
        };
      } catch (error) {
        console.log(error);
        return {
          error: true,
          message: "Success with error. Could not delete file on server.",
        };
      }
    }
    if (info.status === 500) {
      return {
        error: true,
        message: "Server error.",
      };
    }

    return {
      error: true,
      message: await info.text(),
    };
  } catch {
    console.error("Delete request fetch failed.");
    return {
      error: true,
      message: "Connection error.",
    };
  }
}
