"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get rest information of chosen item from invoices and outside items data.
 * @param  {string} currency Name of currency.
 * @param  {Number} itemId Item id.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @return {{outsideItemInfos: Array<{users: Array<{key: Number, value: string}>, organizationName: string, qty: Number, price: Number, currency: string}>, ownedItemInfos: Array<{userId: Number, username: string, organizationName: string, invoiceNumber: string, qty: Number, price: Number, currency: string}>}}      Object containing rest item information. If connection was lost return null.
 */
export default async function getRestInfo(currency, itemId, isOrg) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = "";
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Warehouse/get/rest/org/${itemId}/${currency}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Warehouse/get/rest/${itemId}/${currency}/user/${userId}`;
  }
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return {};
  } catch {
    console.error("getRestInfo fetch failed.");
    return null;
  }
}
