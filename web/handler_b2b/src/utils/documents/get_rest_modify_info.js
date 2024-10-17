"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestModifyInvoice(invoiceId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/rest/modify/${invoiceId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return {};
  } catch {
    console.error("getRestModifyInvoice fetch failed.");
    return null;
  }
}
