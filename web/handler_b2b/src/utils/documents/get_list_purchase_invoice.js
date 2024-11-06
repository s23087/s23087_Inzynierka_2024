"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get purchase invoices list.
 * @return {Promise<Array<{invoiceId: Number, invoiceNumber: string, clientName: string, orgName: string}>>} Array of objects that contain purchase invoice information. If connection was lost return null.
 */
export default async function getListOfPurchaseInvoice() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/purchase/list`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getListOfPurchaseInvoice fetch failed.");
    return null;
  }
}
