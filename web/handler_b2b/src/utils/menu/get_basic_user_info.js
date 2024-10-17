"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

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
    console.error("getBasicInfo fetch failed.")
    return {
      username: "connection error",
      surname: "connection error",
      orgName: "connection error",
    };
  }
}
