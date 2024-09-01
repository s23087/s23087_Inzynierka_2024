"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import { redirect } from "next/navigation";

export default async function getUserRoles(search) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url;
  if (search) {
    url = `${process.env.API_DEST}/${dbName}/Roles/getUserRoles?userId=${userId}&search=${search}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Roles/getUserRoles?userId=${userId}`;
  }
  const data = await fetch(url, {
    method: "GET",
  });

  if (data.status == 404) {
    logout();
    redirect("/");
  }

  if (data.ok) {
    return await data.json();
  }

  return {};
}
