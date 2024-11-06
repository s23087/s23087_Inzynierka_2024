"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get rest information of chosen credit note. If not found return object without properties. Other wise return object with properties:
 *  creditNoteNumber, currencyName, note, path and creditItems.
 * @param creditId Credit note id.
 * @return {Promise<{creditNoteNumber: string, currencyName: string, note: string, path: string, creditItems: Array<{creditItemId: Number, partnumber: string, itemName: string, qty: Number, price: Number}>}>} Object that that contain invoice information. If connection was lost return null.
 */
export default async function getRestCreditNote(creditId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/CreditNote/rest/${creditId}`;
  try {
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
  } catch {
    console.error("Get rest credit note fetch failed.");
    return {
      creditNoteNumber: "Connection error.",
      currencyName: "",
      note: "Connection error.",
      path: "",
      creditItems: [],
    };
  }
}
