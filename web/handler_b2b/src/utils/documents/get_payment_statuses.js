"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get payment statuses lists.
 * @return {Promise<Array<{paymentStatuses: Number, statusName: string}>>} Array of objects that contain payment status information. If connection was lost return null.
 */
export default async function getPaymentStatuses() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/payment/statuses`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getPaymentStatuses fetch failed.");
    return null;
  }
}
