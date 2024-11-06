"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get delivery companies.
 * @return {Promise<Array<{id: Number, name: string}>>} Array of objects that that contain companies information. If connection was lost return null.
 */
export default async function getDeliveryCompany() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Delivery/get/delivery_companies`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getDeliveryCompany fetch failed.");
    return null;
  }
}
