"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

/**
 * Sends request to modify item bindings in database.
 * @param  {Array<{userId: Number, invoiceId: Number, itemId: Number, qty: Number}>} bindings Any array of object containing all bindings values.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function changeBindings(bindings) {
  const userId = await getUserId();
  let data = {
    userId: userId,
    bindings: bindings,
  };
  try {
    const dbName = await getDbName();
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Warehouse/modify/bindings`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status == 500) {
      return {
        error: true,
        completed: true,
        message: "Server error.",
      };
    }

    if (info.status == 404) {
      logout();
      return {
        error: true,
        completed: true,
        message: "This user doesn't exists.",
      };
    }

    if (info.ok) {
      return {
        error: false,
        completed: true,
        message: "Success!",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error",
      };
    }
  } catch {
    console.error("changeBindings fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error",
    };
  }
}
