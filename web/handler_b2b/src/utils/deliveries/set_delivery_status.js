"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to change delivery status.
 * @param  {number} deliveryId Delivery id.
 * @param  {string} statusName Name of the status (Fulfilled, In transport, Delivered with issues, Preparing, Rejected).
 * @return {Promise<Boolean>}      Return true if operation succeed, otherwise false.
 */
export default async function setDeliveryStatus(deliveryId, statusName) {
  const dbName = await getDbName();
  let data = {
    statusName: statusName,
    deliveryId: deliveryId,
  };
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Delivery/modify/status`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    return info.ok;
  } catch {
    console.error("setDeliveryStatus fetch failed.");
    return false;
  }
}
