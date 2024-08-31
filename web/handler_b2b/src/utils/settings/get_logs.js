"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getLogs() {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Settings/logs?userId=${userId}`;
  const data = await fetch(url, {
    method: "GET",
  });

  if (data.ok) {
    return await data.json();
  }

  return {};
}
