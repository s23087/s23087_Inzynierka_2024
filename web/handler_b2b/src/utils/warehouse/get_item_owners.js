"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get array of names and surnames of users that is associated with chosen item.
 * @param  {[Number]} itemId Item id.
 * @return {[Object]}      Array of object that contains information about users. If connection was lost return null.
 */
export default async function getItemOwners(itemId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/get/item_owners/${itemId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return [];
  } catch {
    console.error("getItemOwners fetch failed.");
    return null;
  }
}
