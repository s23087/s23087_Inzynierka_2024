"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get user email.
 * @return {Promise<string>} String containing email. If error return string containing error message.
 */
export default async function getUserEmail() {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/User/get/email/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.text();
    }

    return "Critical error.";
  } catch {
    console.error("getUserEmail fetch failed.");
    return "Connection error.";
  }
}
