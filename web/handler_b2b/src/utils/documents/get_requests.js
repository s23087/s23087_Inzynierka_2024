"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getRequests(isOrg) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Requests/get/recived/${userId}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Requests/get/created/${userId}`;
  }
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.json();
  }

  return [];
}
