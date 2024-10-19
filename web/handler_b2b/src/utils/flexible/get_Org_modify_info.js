"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getOrgModifyInfo() {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Settings/get/modify/rest/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return {};
  } catch {
    console.error("getOrgModifyInfo fetch failed.");
    return null;
  }
}
