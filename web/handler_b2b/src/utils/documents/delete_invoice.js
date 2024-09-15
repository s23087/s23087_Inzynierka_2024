"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function deleteInvoice(invoiceId) {
  const dbName = await getDbName();
  const path = await fetch(
    `${process.env.API_DEST}/${dbName}/Invoices/invoicePath/${invoiceId}`,
    {
      method: "GET",
    },
  );
  if (!path) {
    return {
      error: true,
      message: "File do not exists.",
    };
  }
  let invoicePath = await path.text();
  const userId = await getUserId();

  let url = `${process.env.API_DEST}/${dbName}/Invoices/deleteInvoice/${invoiceId}?userId=${userId}`;
  const info = await fetch(url, {
    method: "Delete",
  });

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
}
