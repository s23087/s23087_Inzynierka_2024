"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getItems(currency, isOrg) {
  let url = "";
  const dbName = await getDbName();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Warehouse/items?currency=${currency}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Warehouse/items?currency=${currency}&userId=${userId}`;
  }
  const items = await fetch(url, {
    method: "GET",
  });

  if (items.ok) {
    return await items.json();
  }

  return {};
}
