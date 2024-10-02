"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function getSearchOutsideItems(role, search) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  if (role === "Admin") {
    url = `${process.env.API_DEST}/${dbName}/OutsideItem/get/items/${userId}?search=${search}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/OutsideItem/get/items?search=${search}`;
  }
  const info = await fetch(url, {
    method: "GET",
  });
  if (info.status === 404) {
    logout();
    return [];
  }

  if (info.ok) {
    return await info.json();
  }

  return [];
}
