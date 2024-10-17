"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestModifyRequest(requestId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Requests/get/modify/rest/${requestId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return {};
  } catch {
    console.error("getRestModifyRequest fetch failed.");
    return null;
  }
}
