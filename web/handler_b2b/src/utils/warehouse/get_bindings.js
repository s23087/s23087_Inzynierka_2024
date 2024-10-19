"use server";

import getDbName from "../auth/get_db_name";

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
