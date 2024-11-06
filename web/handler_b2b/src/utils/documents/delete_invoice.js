"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import { getInvoicePath } from "./get_document_path";

/**
 * Sends request to delete chosen invoice. If user do not exist server will logout them.
 * @param  {Number} invoiceId Invoice id.
 * @param {boolean} isYourInvoice Is invoice type "Your invoices".
 * @return {Promise<{error: boolean, message: string}>} Return action result with message. If error is true that action was unsuccessful.
 */
export default async function deleteInvoice(invoiceId, isYourInvoice) {
  const dbName = await getDbName();
  let invoicePath = await getInvoicePath(invoiceId);
  if (invoicePath === null) {
    return {
      error: true,
      message: "Server error.",
    };
  }
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/delete/${invoiceId}/${isYourInvoice}/${userId}`;
  try {
    const info = await fetch(url, {
      method: "Delete",
    });

    if (info.status === 404) {
      let text = await info.text();
      if (text === "User not found.") {
        logout();
        return {
          error: true,
          message: "Your account does not exsist.",
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
        fs.rmSync(invoicePath);
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
    console.error("Delete invoice fetch failed.");
    return {
      error: true,
      message: "Connection error.",
    };
  }
}
