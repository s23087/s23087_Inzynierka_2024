"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

/**
 * Sends request to delete chosen outside item. If user do not exist server will logout them.
 * @param  {Number} itemId Item id.
 * @param  {Number} orgId Organization id item belong to.
 * @return {Promise<object>}      Object with properties result {bool} and message {string}. If result is true, then item has been successfully deleted. Message is only return when there's error.
 */
export default async function deleteOutsideItem(itemId, orgId) {
  const dbName = await getDbName();
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/OutsideItem/delete/org/${orgId}/item/${itemId}/user/${userId}`;
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
          completed: true,
          message: "Unauthorized.",
        };
      } else {
        return {
          error: true,
          completed: true,
          message: text,
        };
      }
    }

    if (info.ok) {
      return {
        error: false,
        message: "Success!",
      };
    }
    return {
      error: true,
      message: await info.text(),
    };
  } catch {
    console.error("deleteOutsideItem fetch failed.");
    return {
      error: true,
      message: "Connection error.",
    };
  }
}
