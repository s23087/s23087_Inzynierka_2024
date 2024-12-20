"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get rest information of chosen proforma.
 * @param  {boolean} isYourProforma Is proforma type "Yours proformas".
 * @param  {Number} proformaId Proforma id.
 * @return {Promise<{taxes: Number, currencyValue: Number, currencyDate: string, paymentMethod: string, inSystem: boolean, note: string, path: string, items: Array<{creditItemId: Number, partnumber: string, itemName: string, qty: Number, price: Number}>}>} Return object containing proforma object information. If connection is lost return null.
 */
export default async function getRestProforma(isYourProforma, proformaId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Proformas/get/${isYourProforma ? "yours" : "clients"}/rest/${proformaId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getRestProforma fetch failed.");
    return null;
  }
}
