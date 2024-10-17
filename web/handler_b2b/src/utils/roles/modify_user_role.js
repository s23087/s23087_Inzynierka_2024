"use server";

import { redirect } from "next/navigation";
import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function modifyUserRole(choosenUser, state, formData) {
  const userId = await getUserId();
  let data = {
    userId: userId,
    choosenUserId: choosenUser,
    roleName: formData.get("role"),
  };

  try {
    const dbName = await getDbName();
    const info = await fetch(`${process.env.API_DEST}/${dbName}/Roles/modify`, {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    });
  
    if (info.status == 404) {
      let text = await info.text();
      if (text === "User not found") {
        logout();
        redirect("/");
      }
      return {
        error: true,
        message: text,
        completed: true,
      };
    }
  
    if (info.ok) {
      return {
        error: false,
        message: "Success! User role has been changed",
        completed: true,
      };
    } else {
      return {
        error: true,
        message: "Critical error",
        completed: true,
      };
    }
  } catch {
    console.error("modifyUserRole fetch failed.")
    return {
      error: true,
      message: "Connection error.",
      completed: true,
    };
  }
}
