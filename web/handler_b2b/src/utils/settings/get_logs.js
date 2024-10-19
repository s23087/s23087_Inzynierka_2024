"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function getLogs() {
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
