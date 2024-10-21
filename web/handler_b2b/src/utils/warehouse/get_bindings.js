"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get item associations with users.
 * @param  {[String]} currency Name of currency.
 * @param  {[Number]} itemId Item id.
 * @return {[Object]}      Array of object that contains item bindings information. If connection was lost return null.
 */
export default async function getBindings(itemId, currency) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/get/bindings/${itemId}/${currency}`;
  try {
    const desc = await fetch(url, {
      method: "GET",
    });

    if (desc.ok) {
      return await desc.json();
    }

    return [];
  } catch {
    console.error("getBindings fetch failed.");
    return null;
  }
}
