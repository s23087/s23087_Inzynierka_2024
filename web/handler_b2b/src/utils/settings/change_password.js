"use server";

import getUserId from "../auth/get_user_id";
import getDbName from "../auth/get_db_name";
import logout from "../auth/logout";
import { redirect } from "next/navigation";

export default async function ChangePassword(state, formData) {
  if (!formData.get("newPassword") || !formData.get("oldPassword")) {
    return {
      error: true,
      message: "One of input is empty",
      completed: true,
    };
  }
  const userId = await getUserId();
  const dbName = await getDbName();

  let data = {
    userId: userId,
    oldPassword: formData.get("oldPassword"),
    newPassword: formData.get("newPassword"),
  };
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Settings/changePassword`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );
  
    if (info.status === 404) {
      logout();
      redirect("/");
    }
  
    if (info.status === 401) {
      return {
        error: true,
        message: "Password is incorrect.",
        completed: true,
      };
    }
  
    if (info.status === 400) {
      return {
        error: true,
        message: "New password is the same as old one.",
        completed: true,
      };
    }
  
    if (info.ok) {
      return {
        error: false,
        completed: true,
      };
    }
  
    return {
      error: true,
      message: "Ups, something went wrong.",
      completed: true,
    };
  } catch {
    console.error("ChangePassword fetch failed.")
    return {
      error: true,
      message: "Connection error.",
      completed: true,
    };
  }
}
