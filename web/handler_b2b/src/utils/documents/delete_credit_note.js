"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function deleteCreditNote(creditNoteId, isYourCredit) {
  const dbName = await getDbName();
  const path = await fetch(
    `${process.env.API_DEST}/${dbName}/CreditNote/path/${creditNoteId}`,
    {
      method: "GET",
    },
  );
  if (path.status === 404) {
    return {
      error: true,
      message: "Credit note do not exists.",
    };
  }
  if (!path.ok) {
    return {
      error: true,
      message: "Server error. Cloud not download file path.",
    };
  }
  let creditPath = await path.text();
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/CreditNote/delete/${creditNoteId}?userId=${userId}&isYourCredit=${isYourCredit}`;
  const info = await fetch(url, {
    method: "Delete",
  });

  if (info.ok) {
    if (!creditPath) {
      return {
        error: false,
        message: "Success!",
      };
    }
    const fs = require("node:fs");
    try {
      fs.rmSync(creditPath);
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
