"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getSearchYoursInvoices(isOrg, search) {
  let url = "";
  const dbName = await getDbName();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Invoices/getPurchaseInvoicesOrg?search=${search}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Invoices/getPurchaseInvoices?userId=${userId}&search=${search}`;
  }
  const items = await fetch(url, {
    method: "GET",
  });

  if (items.ok) {
    return await items.json();
  }

  return {};
}
