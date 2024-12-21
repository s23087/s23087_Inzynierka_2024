"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to delete chosen item. If user do not exist server will logout them.
 * @param  {Number} itemId Item id.
 * @return {Promise<{result: boolean, message: string}>} If result is true, then item has been successfully deleted. Message is only returned when there's error.
 */
export default async function deleteItem(itemId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/delete/item/${itemId}/${userId}`;
  try {
    const info = await fetch(url, {
      method: "Delete",
    });

    if (info.status === 404) {
      let text = await info.text();
      if (text === "User not found.") logout();
      return {
        result: false,
        message: text,
      };
    }

    if (info.status === 400) {
      let text = await info.text();
      return {
        result: false,
        message: text,
      };
    }

    if (info.status === 500) {
      return {
        result: false,
        message: "Server error",
      };
    }

    if (info.ok) {
      return {
        result: true,
      };
    }

    return {
      result: true,
      message: "Critical error.",
    };
  } catch {
    console.error("deleteItem fetch failed.");
    return {
      result: true,
      message: "Connection error.",
    };
  }
}
