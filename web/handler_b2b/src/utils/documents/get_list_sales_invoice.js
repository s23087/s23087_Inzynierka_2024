"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get sales invoices list.
 * @return {Promise<Array<{invoiceId: Number, invoiceNumber: string, clientName: string, orgName: string}>>} Array of objects that contain sales invoice information. If connection was lost return null.
 */
export default async function getListOfSalesInvoice() {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/sales/list`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getListOfSalesInvoice fetch failed.");
    return null;
  }
}
