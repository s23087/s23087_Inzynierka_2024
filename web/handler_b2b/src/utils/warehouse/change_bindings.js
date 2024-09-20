"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function changeBindings(bindings) {
  const userId = await getUserId();
  let data = {
    userId: userId,
    bindings: bindings
  };

  const dbName = await getDbName();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Warehouse/changeBindings`,
    {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  if (info.status == 500) {
    return {
      error: true,
      completed: true,
      message: "Server error.",
    };
  }

  if (info.status == 404) {
    logout()
    return {
      error: true,
      completed: true,
      message: "This user doesn't exists.",
    };
  }

  if (info.ok) {
    return {
      error: false,
      completed: true,
      message: "Success!"
    };
  } else {
    return {
      error: true,
      completed: true,
      message: "Critical error",
    };
  }
}
