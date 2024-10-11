"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getSearchClients(isOrg, search, sort, country) {
  let url = "";
  const dbName = await getDbName();
  const userId = await getUserId();
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (country) params.push(`country=${country}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Client/orgClients?userId=${userId}&search=${search}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Client/clients?userId=${userId}&search=${search}`;
  }
  try {
    const items = await fetch(url, {
      method: "GET",
    });

    if (items.ok) {
      return await items.json();
    }

    return [];
  } catch {
    return null;
  }
}
