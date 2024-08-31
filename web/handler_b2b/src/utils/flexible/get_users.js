"use server";

import getDbName from "../auth/get_db_name";

export default async function getUsers() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/User/getUsers`;
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.json();
  }

  return {};
}
