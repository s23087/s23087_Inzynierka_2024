"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to delete chosen delivery. If user do not exist server will logout them.
 * @param {boolean} isDeliveryToUser Is delivery type "Deliveries to user".
 * @param  {Number} deliveryId Delivery id.
 * @return {Promise<{error: boolean, message: string}>} Return action result with message. If error is true that action was unsuccessful.
 */
export default async function deleteDelivery(isDeliveryToUser, deliveryId) {
  const dbName = await getDbName();
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/Delivery/delete/${isDeliveryToUser}/${deliveryId}/user/${userId}`;
  try {
    const info = await fetch(url, {
      method: "Delete",
    });

    if (info.ok) {
      return {
        error: false,
        message: "Success!",
      };
    }
    return {
      error: true,
      message: await info.text(),
    };
  } catch {
    console.error("deleteDelivery fetch failed.");
    return {
      error: true,
      message: "Connection error.",
    };
  }
}
