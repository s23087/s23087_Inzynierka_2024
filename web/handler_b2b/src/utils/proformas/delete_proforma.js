"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import getProformaPath from "./get_proforma_path";

/**
 * Sends request to delete proforma.
 * @param  {boolean} isYourProforma Is proforma type "Yours proformas".
 * @param  {Number} proformaId Proforma id.
 * @return {Promise<object>}      Return object containing property error {bool} and message {string}. If error is true that action was unsuccessful.
 */
export default async function deleteProforma(isYourProforma, proformaId) {
  const dbName = await getDbName();
  const path = await getProformaPath(proformaId);
  if (path === null) {
    return {
      error: true,
      message: "Could not download proforma path.",
    };
  }
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/Proformas/delete/${isYourProforma ? "yours" : "clients"}/${proformaId}/user/${userId}`;
  try {
    const info = await fetch(url, {
      method: "Delete",
    });

    if (info.ok) {
      if (path === "") {
        return {
          error: false,
          message: "Success!",
        };
      }
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
  } catch {
    console.error("deleteProforma fetch failed.");
    return {
      error: true,
      message: "Connection error.",
    };
  }
}
