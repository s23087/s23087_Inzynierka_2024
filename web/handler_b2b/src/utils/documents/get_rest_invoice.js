"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get rest information of chosen invoice. If not found return object without properties.
 * @param invoiceId Invoice id.
 * @param isYourInvoice Is invoice type "Yours invoices" boolean.
 * @return {Promise<{tax: Number, currencyValue: Number, currencyName: string, currencyDate: string, transportCost: Number, paymentType: string, note: string, path: string|undefined, creditNotes: Array<string>, items: Array<{priceId: Number, partnumber: string, users: Array<string>, itemName: string, qty: Number, price: Number}>}>} Object that contain invoice information. If connection was lost return null.
 */
export default async function getRestInvoice(invoiceId, isYourInvoice) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let url = "";
  const dbName = await getDbName();
  if (isYourInvoice) {
    url = `${process.env.API_DEST}/${dbName}/Invoices/rest/purchase/${invoiceId}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Invoices/rest/sales/${invoiceId}`;
  }
  try {
    const items = await fetch(url, {
      method: "GET",
    });

    if (items.ok) {
      return await items.json();
    }

    return {};
  } catch {
    console.error("getRestInvoice fetch failed.");
    return null;
  }
}
