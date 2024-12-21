"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get rest information of chosen pricelist.
 * @param  {Number} pricelistId Offer id.
 * @return {Promise<{maxQty: Number, items: Array<{id: Number, partnumber: string, qty: Number, purchasePrice: Number, price: Number}>}>} Return object containing rest information of chosen pricelist. If connection is lost return null.
 */
export default async function getRestModifyPricelist(pricelistId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/rest/${pricelistId}/user/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return {
      maxQty: 0,
      items: [],
    };
  } catch {
    console.error("getRestModifyPricelist fetch failed.");
    return null;
  }
}
