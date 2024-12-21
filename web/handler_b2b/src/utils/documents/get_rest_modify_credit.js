"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get rest information of chosen credit note needed to modify. If not found return object without properties.
 * Otherwise return object with properties: creditNumber, orgName and note.
 * @param {Number} creditId Credit note id.
 * @return {Promise<{creditNumber: string, orgName: string, note: string}>}      Object that contain credit note information. If connection was lost return null.
 */
export default async function getRestModifyCredit(creditId, isYourCredit) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/CreditNote/get/rest/modify/${isYourCredit}/${creditId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return {};
  } catch {
    console.error("getRestModifyCredit fetch failed.");
    return null;
  }
}
