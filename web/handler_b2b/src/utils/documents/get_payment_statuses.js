"use server";

import getDbName from "../auth/get_db_name";

export default async function getPaymentStatuses() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/getPaymentStatuses`;
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.json();
  }

  return [];
}
