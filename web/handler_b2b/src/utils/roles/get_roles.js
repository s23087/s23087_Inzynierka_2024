"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get accessible roles.
 * @return {Promise<Array<string>>} Array of objects that contain role information. If connection was lost return null.
 */
export default async function getRoles() {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Roles/get`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return [];
  } catch {
    console.error("getRoles fetch failed.");
    return null;
  }
}
