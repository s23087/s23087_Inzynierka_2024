"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getNotifications() {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Notifications/getNotifications/${userId}`;
  const desc = await fetch(url, {
    method: "GET",
  });

  if (desc.ok) {
    return await desc.json();
  }

  return [];
}
