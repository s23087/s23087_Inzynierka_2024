"use server";

import getDbName from "../auth/get_db_name";

export default async function getItemOwners(itemId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/getItemOwners/${itemId}`;
  const data = await fetch(url, {
    method: "GET",
  });

  if (data.ok) {
    return await data.json();
  }

  return [];
}
