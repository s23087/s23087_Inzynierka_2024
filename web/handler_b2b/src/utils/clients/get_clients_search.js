"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getSearchClients(isOrg, search) {
  let url = "";
  const dbName = await getDbName();
  const userId = await getUserId();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Client/orgClients?userId=${userId}&search=${search}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Client/clients?userId=${userId}&search=${search}`;
  }
  const items = await fetch(url, {
    method: "GET",
  });

  if (items.ok) {
    return await items.json();
  }

  return {};
}
