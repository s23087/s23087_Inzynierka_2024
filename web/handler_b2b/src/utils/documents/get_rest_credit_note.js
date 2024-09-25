"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestCreditNote(creditId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/CreditNote/rest/${creditId}`;
  const items = await fetch(url, {
    method: "GET",
  });

  if (items.status === 404) {
    return {
      creditNoteNumber: "Error look notes",
      currencyName: "",
      note: "Error, this document does not exist.",
      path: "",
      creditItems: [],
    };
  }

  if (items.ok) {
    return await items.json();
  }

  return {
    creditNoteNumber: "Critical error.",
    currencyName: "",
    note: "Critical error.",
    path: "",
    creditItems: [],
  };
}
