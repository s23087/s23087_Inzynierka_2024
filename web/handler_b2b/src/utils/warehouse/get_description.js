"use server";

import getDbName from "../auth/get_db_name";

export default async function getDescription(itemId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/get/description/${itemId}`;
  try {
    const desc = await fetch(url, {
      method: "GET",
    });

    if (desc.ok) {
      return await desc.text();
    }

    return "";
  } catch {
    console.error("getDescription fetch failed.")
    return null;
  }
}
