"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestInfo(currency, itemId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/getRestInfo?itemId=${itemId}&currency=${currency}`;
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.json();
  }

  return {};
}
