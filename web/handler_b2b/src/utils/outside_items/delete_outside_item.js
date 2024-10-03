"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function deleteOutsideItem(itemId, orgId) {
  const dbName = await getDbName();
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/OutsideItem/delete/org/${orgId}/item/${itemId}?userId=${userId}`;
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
}
