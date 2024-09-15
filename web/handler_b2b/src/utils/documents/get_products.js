"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getItemsList(userId, currency) {
  const dbName = await getDbName();
  let url = "";
  if (userId && currency) {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Invoices/allSalesItems/${userId}?currency=${currency}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Invoices/allItems`;
  }
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.json();
  }

  return {};
}
