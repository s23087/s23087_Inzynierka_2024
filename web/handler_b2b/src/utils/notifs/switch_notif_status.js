"use server";

import getDbName from "../auth/get_db_name";

export default async function switchNotifStatus(notifId, notifBool) {
  const dbName = await getDbName();
  try {
    await fetch(
      `${process.env.API_DEST}/${dbName}/Notifications/modify/${notifId}/is_read_/${!notifBool}`,
      {
        method: "POST",
      },
    );
  } catch {
    console.error("switchNotifStatus fetch failed.");
  }
}
