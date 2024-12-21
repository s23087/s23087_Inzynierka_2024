"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get item associations with users.
 * @param  {string} currency Name of currency.
 * @param  {Number} itemId Item id.
 * @return {Promise<Array<{userId: Number|undefined, username: string|undefined, qty: Number, price: Number, currency: string, invoiceNumber: string, invoiceId: Number}>>}      Array of object that contains item bindings information. If connection was lost return null.
 */
export default async function getBindings(itemId, currency) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/get/bindings/${itemId}/${currency}`;
  try {
    const desc = await fetch(url, {
      method: "GET",
    });

    if (desc.ok) {
      return await desc.json();
    }

    return [];
  } catch {
    console.error("getBindings fetch failed.");
    return null;
  }
}
