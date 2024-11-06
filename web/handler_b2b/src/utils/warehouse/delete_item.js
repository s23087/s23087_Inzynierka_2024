"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

/**
 * Sends request to delete chosen item. If user do not exist server will logout them.
 * @param  {Number} itemId Item id.
 * @return {Promise<{result: boolean, message: string}>} If result is true, then item has been successfully deleted. Message is only returned when there's error.
 */
export default async function deleteItem(itemId) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/delete/item/${itemId}/${userId}`;
  try {
    const info = await fetch(url, {
      method: "Delete",
    });

    if (info.status == 404) {
      let text = await info.text();
      if (text === "User not found.") logout();
      return {
        result: false,
        message: text,
      };
    }

    if (info.status == 400) {
      let text = await info.text();
      return {
        result: false,
        message: text,
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
