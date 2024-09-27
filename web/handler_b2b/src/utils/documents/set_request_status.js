"use server";

import getDbName from "../auth/get_db_name";

export default async function setRequestStatus(requestId, statusName, note) {
  const dbName = await getDbName();
  let data = {
    statusName: statusName,
    note: note,
  };
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Requests/modify/${requestId}/status`,
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
