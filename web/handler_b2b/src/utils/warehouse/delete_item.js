"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function deleteItem(itemId) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/deleteItem?itemId=${itemId}&userId=${userId}`;
  const info = await fetch(url, {
    method: "Delete",
  });

  return info.ok;
}
