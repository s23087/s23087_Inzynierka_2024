"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get application logs. Return empty array when encounters the error. When 404 error is encounter it will logout user.
 * @return {Promise<Array<{logType: string, logDescription: string, logDate: string, username: string, surname: string}>}      Array of objects that contain logs information. If connection was lost return null.
 */
export default async function getLogs() {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Settings/get/logs/${userId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.status === 404) {
      logout();
      return [];
    }

    if (data.status === 401) {
      return [];
    }

    if (data.ok) {
      return await data.json();
    }

    return [];
  } catch {
    console.error("getLogs fetch failed.");
    return null;
  }
}
