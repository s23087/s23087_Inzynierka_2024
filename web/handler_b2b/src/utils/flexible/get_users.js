"use server";

import getDbName from "../auth/get_db_name";
import getRole from "../auth/get_role";
import getUserId from "../auth/get_user_id";

export default async function getUsers() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/User/get/users`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      let result = await info.json();
      const role = await getRole();
      const userId = await getUserId();
      if (role === "Merchant") {
        result = Object.values(result).filter((e) => e.idUser === userId);
      }
      return result;
    }

    return [];
  } catch {
    console.error("getUsers fetch failed.");
    return null;
  }
}
