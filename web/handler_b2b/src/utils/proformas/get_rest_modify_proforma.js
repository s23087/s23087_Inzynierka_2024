"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get rest information of chosen proforma needed for modification.
 * @param  {Number} proformaId Proforma id.
 * @return {Promise<{status: boolean, paymentMethod: string, note: string}>} Return object containing proforma object information.
 * If connection is lost or proforma is not found return error message in attribute payment method and note.
 */
export default async function getRestModifyProforma(proformaId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Proformas/get/rest/modify/${proformaId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return {
      status: false,
      paymentMethod: "Not found",
      note: "Not found",
    };
  } catch {
    console.error("getRestModifyProforma fetch failed.");
    return {
      status: false,
      paymentMethod: "Connection error.",
      note: "Connection error.",
    };
  }
}
