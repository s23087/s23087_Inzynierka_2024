"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getCreditNotes(isOrg, isYourInvoice) {
  let url = "";
  const dbName = await getDbName();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/CreditNote/getCreditNote/${isYourInvoice}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/CreditNote/getCreditNote/${isYourInvoice}/${userId}`;
  }
  const items = await fetch(url, {
    method: "GET",
  });

  if (items.ok) {
    return await items.json();
  }

  return [];
}
