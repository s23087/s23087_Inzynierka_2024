"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get rest information of chosen delivery. If not found return object without properties. Other wise return object with properties:
 *  noteDate, username and note.
 * @param {Number} deliveryId Delivery id.
 * @return {Promise<{note: Array<{noteDate: string, username: string, note: string}>, item: Array<{itemName: string, partnumber: string, qty: Number}>}>} Object that that contain delivery information. If connection was lost return null.
 */
export default async function getRestDelivery(deliveryId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Delivery/get/rest/${deliveryId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return {};
  } catch {
    console.error(" fetch failed.");
    return null;
  }
}
