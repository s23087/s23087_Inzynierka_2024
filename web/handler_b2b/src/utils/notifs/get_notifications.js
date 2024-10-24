"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get notifications.
 * @return {Promise<Array<Object>>}      Array of objects that contain notifications information. If connection was lost return null.
 */
export default async function getNotifications() {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Notifications/get/${userId}`;
  try {
    const desc = await fetch(url, {
      method: "GET",
    });

    if (desc.ok) {
      return await desc.json();
    }

    return [];
  } catch {
    console.error("getNotifications fetch failed.");
    return null;
  }
}
