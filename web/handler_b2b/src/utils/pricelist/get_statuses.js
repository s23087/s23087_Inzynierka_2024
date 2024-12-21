"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get offer statuses.
 * @return {Promise<Array<{statusId: string, statusName: string}>>} Array of object that contains offer statuses information. If connection was lost return null.
 */
export default async function getOfferStatuses() {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/statues`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getOfferStatuses fetch failed.");
    return null;
  }
}
