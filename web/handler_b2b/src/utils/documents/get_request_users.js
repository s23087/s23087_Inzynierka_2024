"use server";

import getDbName from "../auth/get_db_name";

export default async function getReceviers() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Requests/get/receviers`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getReceviers fetch failed.");
    return null;
  }
}
