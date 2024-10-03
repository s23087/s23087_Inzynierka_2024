"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getBasicInfo() {
  const dbName = await getDbName();
  const userId = await getUserId();
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/User/basicInfo/${userId}`,
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
    return {
      username: "error",
      surname: "error",
      orgName: "error",
    };
  }
}
