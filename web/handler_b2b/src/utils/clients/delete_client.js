"use server";

import { redirect } from "next/navigation";
import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function deleteClient(orgId) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Client/delete/${orgId}/${userId}`;
  const info = await fetch(url, {
    method: "DELETE",
  });

  if (info.status == 404) {
    let text = await info.text()
    if (text === "User not found.") {
      logout();
      return {
        error: true,
        message:
          "Your account does not exists.",
      };
    }
    return {
      error: true,
      message: text,
    };
  }

  if (info.status == 400) {
    return {
      error: true,
      message: await info.text(),
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
