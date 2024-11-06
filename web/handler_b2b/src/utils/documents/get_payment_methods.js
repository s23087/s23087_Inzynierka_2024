"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get payment method lists.
 * @return {Promise<Array<{paymentMethodId: Number, methodName: string}>>} Array of objects that contain payment information. If connection was lost return null.
 */
export default async function getPaymentMethods() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/payment/methods`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getPaymentMethods fetch failed.");
    return null;
  }
}
