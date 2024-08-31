"use server";

import { redirect } from "next/navigation";
import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function deleteClient(orgId) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Client/deleteOrg/${orgId}?userId=${userId}`;
  const info = await fetch(url, {
    method: "DELETE",
  });

  console.log(info.status);

  if (info.status == 404) {
    logout();
    redirect("/");
  }

  if (info.status == 400) {
    return {
      error: true,
      message:
        "This object have relation. Please delete all binded objects to this enitity.",
    };
  }
  if (info.ok) {
    return {
      error: false,
      message: "Success!",
    };
  }

  return {
    error: true,
    message: "Critical error.",
  };
}
