"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to create delivery company.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function createDeliveryCompany(state, formData) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let companyName = formData.get("name");
  if (!companyName)
    return {
      error: true,
      completed: true,
      message: "Company name must not be empty or exceed 40 chars.",
    };

  const userId = await getUserId();
  const dbName = await getDbName();

  let data = {
    companyName: companyName,
  };

  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Delivery/add/company/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );
    if (info.status === 404) {
      let text = await info.text();
      if (text === "User not found.") {
        logout();
      }
      return {
        error: true,
        completed: true,
        message: "Your user profile does not exists.",
      };
    }
    if (info.status === 400) {
      return {
        error: true,
        completed: true,
        message: await info.text(),
      };
    }

    if (info.status === 500) {
      return {
        error: true,
        completed: true,
        message: "Server error.",
      };
    }

    if (info.ok) {
      return {
        error: false,
        completed: true,
        message: "Success! You had added new company.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error.",
      };
    }
  } catch {
    console.error("createDeliveryCompany fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
