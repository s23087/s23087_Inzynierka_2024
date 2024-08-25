"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getNotificationCounter() {
  const dbName = await getDbName();
  const userId = await getUserId();
  const notifCount = await fetch(
    `${process.env.API_DEST}/${dbName}/User/notificationCount/${userId}`,
    { method: "GET" },
  ).then((res) => res.json());

  return notifCount;
}
