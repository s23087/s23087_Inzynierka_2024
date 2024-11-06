"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get request status lists.
 * @return {Promise<Array<{id: Number, name: string}>>} Array of objects that contain status information. If connection was lost return null.
 */
export default async function getRequestStatuses() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Requests/get/statuses`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch (error) {
    console.error("getRequestStatuses fetch failed.");
    return null;
  }
}
