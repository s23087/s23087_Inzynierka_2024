"use server";

import getDbName from "../auth/get_db_name";

export default async function getListOfPurchaseInvoice() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/purchase/list`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getListOfPurchaseInvoice fetch failed.");
    return null;
  }
}
