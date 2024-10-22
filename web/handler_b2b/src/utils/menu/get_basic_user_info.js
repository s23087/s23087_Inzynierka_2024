"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get basic info of current user.
 * @return {Promise<object>} Object containing username, surname and orgName of user. If error occur it's message will be passed.
 */
export default async function getBasicInfo() {
  const dbName = await getDbName();
  const userId = await getUserId();
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/User/info/${userId}`,
      { method: "GET" },
    );
    if (info.ok) {
      return await info.json();
    } else {
      return {
        username: "error",
        surname: "error",
        orgName: "error",
      };
    }
  } catch {
    console.error("getBasicInfo fetch failed.");
    return {
      username: "connection error",
      surname: "connection error",
      orgName: "connection error",
    };
  }
}
