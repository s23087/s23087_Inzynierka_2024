"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get pricelsit items.
 * @param  {Number} pricelistId Offer id.
 * @return {Promise<Array<Object>>}      Return array of object containing pricelist items. If connection is lost return null.
 */
export default async function getPricelistItems(pricelistId) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/items/pricelist/${pricelistId}/user/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getPricelistItems fetch failed.");
    return null;
  }
}
