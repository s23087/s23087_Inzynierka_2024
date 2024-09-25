"use server";

import getDbName from "../auth/get_db_name";

export default async function getCreditPath(creditId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/CreditNote/path/${creditId}`;
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.text();
  }

  return null;
}
