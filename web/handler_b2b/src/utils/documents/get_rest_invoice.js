"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestInvoice(invoiceId, isYourInvoice) {
  let url = "";
  const dbName = await getDbName();
  if (isYourInvoice) {
    url = `${process.env.API_DEST}/${dbName}/Invoices/rest/purchase/${invoiceId}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Invoices/rest/sales/${invoiceId}`;
  }
  const items = await fetch(url, {
    method: "GET",
  });

  if (items.ok) {
    return await items.json();
  }

  return {};
}
