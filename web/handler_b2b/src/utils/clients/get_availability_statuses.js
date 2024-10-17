"use server";

import getDbName from "../auth/get_db_name";

export default async function getAvailabilityStatuses() {
  const dbName = await getDbName();
  try {
    let url = `${process.env.API_DEST}/${dbName}/Client/get/availability_statuses`;
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("Get availability statuses fetch failed.");
    return null;
  }
}
