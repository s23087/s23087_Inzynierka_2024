"use server";

import getDbName from "../auth/get_db_name";

export default async function getUserClientBindings(orgId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Client/getUserClientBindings?orgId=${orgId}`;
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.json();
  }

  return [];
}
