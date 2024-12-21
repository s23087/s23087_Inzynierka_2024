"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get user organization name.
 * @return {Promise<string>}      Return string that contains user organization name. If connection is lost return null.
 */
export default async function getUserOrgName() {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/User/get/organization/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.status === 404) {
      logout();
      return null;
    }

    if (info.ok) {
      return await info.text();
    }

    return "";
  } catch {
    console.error("getUserOrgName fetch failed.");
    return null;
  }
}
