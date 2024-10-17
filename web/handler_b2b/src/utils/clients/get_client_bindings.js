"use server";

import getDbName from "../auth/get_db_name";

export default async function getUserClientBindings(orgId) {
  const dbName = await getDbName();
  try {
    let url = `${process.env.API_DEST}/${dbName}/Client/get/user_client_bindings/${orgId}`;
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.status === 404) {
      return null;
    }

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("Get user client bindings fetch failed.");
    return null;
  }
}
