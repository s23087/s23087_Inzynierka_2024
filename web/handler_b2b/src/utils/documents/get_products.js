"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get available to sell user items if user id and currency are passed. Otherwise return list of items.
 * @param {string} userId User id.
 * @param {string} currency Shortcut name of currency.
 * @return {Promise<Array<{itemId: Number, priceId: Number, invoiceId: Number, invoiceNumber: string, partnumber: string, name: string, qty: Number, price: Number}|{id: Number, partnumber: string, name: string}>>} Array of objects that contain items information. If connection was lost return null. If org view is true return first option, otherwise second.
 */
export default async function getItemsList(userId, currency) {
  const dbName = await getDbName();
  let url = "";
  if (userId && currency) {
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/sales/items/${userId}/currency/${currency}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/items`;
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
    console.error("getItemsList fetch failed.");
    return null;
  }
}
