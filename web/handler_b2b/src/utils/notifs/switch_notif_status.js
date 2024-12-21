"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to change notification status.
 * @param  {Number} notifId Notification id.
 * @param  {Number} notifBool Notification current bool value.
 */
export default async function switchNotifStatus(notifId, notifBool) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  try {
    await fetch(
      `${process.env.API_DEST}/${dbName}/Notifications/modify/${notifId}/is_read/${!notifBool}`,
      {
        method: "POST",
      },
    );
  } catch {
    console.error("switchNotifStatus fetch failed.");
  }
}
