"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function getSearchClients(isOrg, search, sort, country) {
  let url = "";
  const dbName = await getDbName();
  const userId = await getUserId();
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (country) params.push(`country=${country}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Client/get/org/${userId}?search=${search}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Client/get/${userId}?search=${search}`;
  }
  try {
    const items = await fetch(url, {
      method: "GET",
    });

    if (items.status === 404) {
      logout();
      return [];
    }

    if (items.ok) {
      return await items.json();
    }

    return [];
  } catch {
    console.error("Get search client fetch failed.");
    return null;
  }
}
