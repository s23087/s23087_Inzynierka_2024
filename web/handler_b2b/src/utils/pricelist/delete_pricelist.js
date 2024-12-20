"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to delete pricelist. If user is not found in database server will logout the user.
 * @param  {Number} pricelistId Pricelist id.
 * @param  {string} path Pricelist file path.
 * @return {Promise<{error: boolean, message: string}>} Return action result with message. If error is true that action was unsuccessful.
 */
export default async function deletePricelist(pricelistId, path) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/Offer/delete/${pricelistId}/user/${userId}`;
  try {
    const info = await fetch(url, {
      method: "Delete",
    });

    if (info.status === 404) {
      let text = await info.text();
      if (text === "User not found.") {
        logout();
        return {
          error: true,
          message: "Your account does not exist.",
        };
      }
      return {
        error: true,
        message: text,
      };
    }

    if (info.ok) {
      const fs = require("node:fs");
      try {
        fs.rmSync(path);
        return {
          error: false,
          message: "Success!",
        };
      } catch (error) {
        console.log(error);
        return {
          error: true,
          message: "Success with error. Could not delete file on server.",
        };
      }
    }
    return {
      error: true,
      message: "Critical error.",
    };
  } catch {
    console.error("deletePricelist fetch failed.");
    return {
      error: true,
      message: "Connection error.",
    };
  }
}
