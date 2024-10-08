"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function deletePricelist(pricelistId, path) {
  const dbName = await getDbName();
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/Offer/delete/${pricelistId}?userId=${userId}`;
  const info = await fetch(url, {
    method: "Delete",
  });

  if (info.status === 404) {
    let text = await info.text();
    if (text === "User not found.") {
      logout();
      return {
        error: true,
        message: "Your account doe not exist.",
      };
    }
    return {
      error: true,
      message: text,
    };
  }

  if (info.ok) {
    const fs = require("node:fs");
    try {
      fs.rmSync(path);
      return {
        error: false,
        message: "Success!",
      };
    } catch (error) {
      console.log(error);
      return {
        error: true,
        message: "Success with error. Could not delete file on server.",
      };
    }
  }
  return {
    error: true,
    message: await info.text(),
  };
}
