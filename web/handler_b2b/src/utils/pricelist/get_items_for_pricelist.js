"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get available items for proforma to add.
 * @param {string} currency Shortcut name of currency.
 * @return {Promise<Array<Object>>}      Return array of objects containing available items to add in proforma. If connection is lost return null.
 */
export default async function getItemsForPricelist(currency) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/items/${currency}/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getItemsForPricelist fetch failed.");
    return null;
  }
}
