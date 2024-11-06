"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get delivery statuses.
 * @return {Promise<Array<{id: Number, name: string}}>>} Array of objects that that contain status information. If connection was lost return null.
 */
export default async function getDeliveryStatuses() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Delivery/get/delivery_statuses`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getDeliveryStatuses fetch failed.");
    return null;
  }
}
