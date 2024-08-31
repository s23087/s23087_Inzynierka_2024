"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getClients(isOrg) {
  let url = "";
  const userId = await getUserId();
  const dbName = await getDbName();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Client/orgClients?userId=${userId}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Client/clients?userId=${userId}`;
  }
  const items = await fetch(url, {
    method: "GET",
  });

  if (items.ok) {
    return await items.json();
  }

  return {};
}
