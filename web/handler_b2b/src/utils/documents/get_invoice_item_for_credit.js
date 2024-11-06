"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get chosen invoice item list.
 * @param {Number} invoiceId Invoice id.
 * @param {boolean} isYourInvoice Is invoice type "Yours invoices".
 * @return {Promise<Array<{priceId: Number, invoiceId: Number, itemId: Number, itemName: string, partnumber: string, qty: Number, price: Number}>>} Array of objects that contain item information. If connection was lost return null.
 */
export default async function getInvoiceItemForCredit(
  invoiceId,
  isYourInvoice,
) {
  const dbName = await getDbName();
  let url;
  if (isYourInvoice) {
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/purchase/items/${invoiceId}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/sales/items/${invoiceId}`;
  }
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("Get invoice item for credit fetch failed.");
    return null;
  }
}
