"use server";

import getDbName from "../auth/get_db_name";

export default async function getPaymentMethods() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/payment/methods`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getPaymentMethods fetch failed.");
    return null;
  }
}
