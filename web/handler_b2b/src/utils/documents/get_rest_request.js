"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get rest information of chosen request. If not found return object without properties. Other wise return object with properties path and note.
 * @param requestId Request id.
 * @return {Promise<{path: string|undefined, note: string}>} Object that contain request information. If connection was lost return null.
 */
export default async function getRestRequest(requestId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Requests/get/rest/${requestId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return {};
  } catch {
    console.error("getRestRequest fetch failed.");
    return null;
  }
}
