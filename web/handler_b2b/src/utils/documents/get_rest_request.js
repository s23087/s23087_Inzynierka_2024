"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestRequest(requestId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Requests/get/rest/${requestId}`;
  const data = await fetch(url, {
    method: "GET",
  });

  if (data.ok) {
    return await data.json();
  }

  return {};
}
