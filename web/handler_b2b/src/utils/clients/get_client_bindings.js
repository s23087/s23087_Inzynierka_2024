"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get chosen organization user bindings.
 * @param {Number} orgId Organization id
 * @return {Promise<Array<{idUser: Number, username: string, surname: string}>>} Array of objects that that contain client-user binding information. If connection was lost return null.
 */
export default async function getUserClientBindings(orgId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  try {
    let url = `${process.env.API_DEST}/${dbName}/Client/get/user_client_bindings/${orgId}`;
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.status === 404) {
      return null;
    }

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("Get user client bindings fetch failed.");
    return null;
  }
}
