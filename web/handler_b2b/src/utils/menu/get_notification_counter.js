"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get number of user notifications.
 * @return {Promise<Number>}      Number of user notifications.
 */
export default async function getNotificationCounter() {
  const dbName = await getDbName();
  const userId = await getUserId();
  try {
    const notifCount = await fetch(
      `${process.env.API_DEST}/${dbName}/User/get/notification_count/${userId}`,
      { method: "GET" },
    ).then((res) => res.json());

    return notifCount;
  } catch {
    console.error("getNotificationCounter fetch failed.");
    return 0;
  }
}
