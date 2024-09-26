"use server";

import getDbName from "../auth/get_db_name";

export default async function setRequestStatus(statusName) {
  const dbName = await getDbName();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Requests/modify/${requestId}/status/${statusName.replace(" ", "%20")}`,
    {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  return info.ok;
}
