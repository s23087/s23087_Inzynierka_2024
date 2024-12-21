"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import { getCreditPath } from "./get_document_path";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to delete chosen credit note. If user do not exist server will logout them.
 * @param  {Number} creditNoteId Credit note id.
 * @param {boolean} isYourCredit Is invoice type "Your credit notes".
 * @return {Promise<{error: boolean, message: string}>} Return action result with message. If error is true that action was unsuccessful.
 */
export default async function deleteCreditNote(creditNoteId, isYourCredit) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let creditPath = await getCreditPath(creditNoteId);
  if (creditPath === null) {
    return {
      error: true,
      message: "Server error.",
    };
  }
  try {
    const userId = await getUserId();
    let url = `${process.env.API_DEST}/${dbName}/CreditNote/delete/${creditNoteId}/${isYourCredit}/${userId}`;
    const info = await fetch(url, {
      method: "Delete",
    });

    if (info.ok) {
      if (!creditPath) {
        return {
          error: false,
          message: "Success!",
        };
      }
      const fs = require("node:fs");
      try {
        fs.rmSync(creditPath);
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
  } catch {
    console.error("Delete credit note fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
