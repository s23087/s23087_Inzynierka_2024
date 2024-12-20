"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to create item in database. If user do not exist server will logout them.
 * @param  {Array<string>} eans Any array of string containing ean values.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function createItem(eans, state, formData) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const userId = await getUserId();
  let data = {
    userId: userId,
    itemName: formData.get("name"),
    itemDescription: formData.get("description") ?? "",
    partNumber: formData.get("partNumber"),
    eans: eans,
  };

  if (
    !validators.lengthSmallerThen(data.itemName, 250) ||
    !validators.stringIsNotEmpty(data.itemName)
  )
    return {
      error: true,
      completed: true,
      message: "Item name must not be empty or exceed 250 chars.",
    };
  if (!validators.lengthSmallerThen(data.itemDescription ?? "", 500))
    return {
      error: true,
      completed: true,
      message: "Item description must not exceed 500 chars.",
    };
  if (
    !validators.lengthSmallerThen(data.partNumber, 150) ||
    !validators.stringIsNotEmpty(data.partNumber)
  )
    return {
      error: true,
      completed: true,
      message: "Item partnumber must not be empty or exceed 150 chars.",
    };

  const dbName = await getDbName();
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Warehouse/add/item`,
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
        return {
          error: true,
          completed: true,
          message: "Your account does not exists.",
        };
      }
      return {
        error: true,
        completed: true,
        message: text,
      };
    }

    if (info.status === 400) {
      let text = await info.text();
      return {
        error: true,
        completed: true,
        message: text,
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
        message: "Success! You have created the item.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error.",
      };
    }
  } catch {
    console.error("createItem fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
