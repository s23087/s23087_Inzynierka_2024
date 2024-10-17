"use server";

import getDbName from "../auth/get_db_name";

export default async function getInvoiceItemForCredit(
  invoiceId,
  isYourInvoice,
) {
  const dbName = await getDbName();
  let url;
  if (isYourInvoice) {
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/purchase/items/${invoiceId}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/sales/items/${invoiceId}`;
  }
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("Get invoice item for credit fetch failed.");
    return null;
  }
}
