"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get available items for pricelist to add.
 * @param {string} currency Shortcut name of currency.
 * @return {Promise<Array<{itemId: Number, partnumber: string, qty: Number, purchasePrice: Number}>>} Return array of objects containing available items to add in pricelist. If connection is lost return null.
 */
export default async function getItemsForPricelist(currency) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/items/${currency}/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getItemsForPricelist fetch failed.");
    return null;
  }
}
