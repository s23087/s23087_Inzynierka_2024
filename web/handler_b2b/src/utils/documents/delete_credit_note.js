"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import { getCreditPath } from "./get_document_path";

export default async function deleteCreditNote(creditNoteId, isYourCredit) {
  const dbName = await getDbName();
  let creditPath = await getCreditPath(creditNoteId);
  if (creditPath === null) {
    return {
      error: true,
      message: "Server error.",
    };
  }
  try {
    const userId = await getUserId();
    let url = `${process.env.API_DEST}/${dbName}/CreditNote/delete/${creditNoteId}/${isYourCredit}/${userId}`;
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
  } catch {
    console.error("Delete credit note fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
