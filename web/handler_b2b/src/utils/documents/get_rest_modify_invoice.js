"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get rest information of chosen invoice needed to modify. If not found return object without properties.
 * Otherwise return object with properties: transport, paymentMethod and note.
 * @param {Number} invoiceId Invoice id.
 * @return {Promise<{transport: Number, paymentMethod: string, note: string}>} Object that contain invoice information. If connection was lost return null.
 */
export default async function getRestModifyInvoice(invoiceId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
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
