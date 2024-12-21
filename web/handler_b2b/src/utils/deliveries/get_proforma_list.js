"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get proforma's without existing delivery. The object in array have properties id and proformaNumber.
 * @param  {boolean} isDeliveryToUser If delivery is of type "Deliveries to user" it value is true, otherwise false.
 * @return {Promise<Array<{id: Number, proformaNumber: string}>>} Array of objects that that contain proforma information. If connection was lost return null.
 */
export default async function getProformaListWithoutDelivery(isDeliveryToUser) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Delivery/get/${isDeliveryToUser ? "user" : "client"}/proformas/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.status === 404) {
      logout();
      return [];
    }

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getProformaListWithoutDelivery fetch failed.");
    return null;
  }
}
