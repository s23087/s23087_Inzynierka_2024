"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get rest information of chosen client. If not found return object without properties.
 * @param {Number} orgId Organization id.
 * @return {Promise<{creditLimit: Number|undefined, availability: string|undefined, daysForRealization: Number|undefined}>} Object that that contain client information. If connection was lost return null.
 */
export default async function getRestClientInfo(orgId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  try {
    let url = `${process.env.API_DEST}/${dbName}/Client/get/rest/${orgId}`;
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.status === 404) {
      return {
        creditLimit: null,
        availability: "This object do not exist",
        daysForRealization: null,
      };
    }

    if (info.ok) {
      return await info.json();
    }

    return {};
  } catch {
    console.error("Get rest client info fetch failed.");
    return null;
  }
}
