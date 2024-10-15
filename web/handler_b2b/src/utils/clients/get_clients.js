"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getClients(isOrg, sort, country) {
  let url = "";
  const userId = await getUserId();
  const dbName = await getDbName();
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (country) params.push(`country=${country}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Client/get/org/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Client/get/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
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
