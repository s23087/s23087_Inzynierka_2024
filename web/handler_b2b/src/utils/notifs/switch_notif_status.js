"use server";

import getDbName from "../auth/get_db_name";

export default async function switchNotifStatus(notifId, notifBool) {
  const dbName = await getDbName();
  await fetch(
    `${process.env.API_DEST}/${dbName}/Notifications/setNotification/${notifId}?isRead=${!notifBool}`,
    {
      method: "POST",
    },
  );
}
