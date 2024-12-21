"use server";

import getUserId from "../auth/get_user_id";
import getDbName from "../auth/get_db_name";
import logout from "../auth/logout";
import { redirect } from "next/navigation";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to modify user password.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function ChangePassword(state, formData) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
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

    if (info.status === 500) {
      return {
        error: true,
        message: "Server error.",
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
    console.error("ChangePassword fetch failed.");
    return {
      error: true,
      message: "Connection error.",
      completed: true,
    };
  }
}
