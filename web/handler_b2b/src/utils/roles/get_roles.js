"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get accessible roles.
 * @return {Promise<Array<Object>>}      Array of objects that contain role information. If connection was lost return null.
 */
export default async function getRoles() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Roles/get`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return [];
  } catch {
    console.error("getRoles fetch failed.");
    return null;
  }
}
