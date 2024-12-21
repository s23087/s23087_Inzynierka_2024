"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to change request status.
 * @param  {number} requestId request id.
 * @param  {string} statusName Name of the status (Fulfilled, Request cancelled, In progress).
 * @param  {string} note User note.
 * @return {Promise<Boolean>}      Return true if operation succeed, otherwise false.
 */
export default async function setRequestStatus(requestId, statusName, note) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let data = {
    statusName: statusName,
    note: note,
  };
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Requests/modify/${requestId}/status`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );
    return info.ok;
  } catch {
    console.error("setRequestStatus fetch failed.");
    return false;
  }
}
