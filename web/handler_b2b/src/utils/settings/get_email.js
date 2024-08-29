"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getUserEmail() {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/User/getEmail/${userId}`;
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.text();
  }

  return "Critical error";
}
