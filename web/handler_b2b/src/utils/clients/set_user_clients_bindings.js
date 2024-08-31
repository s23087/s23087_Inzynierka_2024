"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
export default async function setUserClientBindings(orgId, userIds) {
  let data = {
    orgId: orgId,
    usersId: userIds,
  };

  const dbName = await getDbName();
  const userId = await getUserId();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Client/setUserClientBindings?userId=${userId}`,
    {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  console.log(data);
  console.log(info.status);

  if (info.ok) {
    return {
      ok: true,
      message: "Success!",
    };
  } else {
    let text = await info.text();
    return {
      ok: false,
      message: text,
    };
  }
}
