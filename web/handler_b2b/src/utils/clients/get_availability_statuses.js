"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get organization availability statuses.
 * @return {Promise<Array<{id: Number, name: string, days: Number}>>} Array of objects that that contain status information. If connection was lost return null.
 */
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
