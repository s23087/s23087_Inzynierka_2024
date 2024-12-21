"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get a description of chosen item.
 * @param  {Number} itemId Item id.
 * @return {Promise<string>}     String containing description. If connection was lost return null.
 */
export default async function getDescription(itemId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Warehouse/get/description/${itemId}`;
  try {
    const desc = await fetch(url, {
      method: "GET",
    });

    if (desc.ok) {
      return await desc.text();
    }

    return "";
  } catch {
    console.error("getDescription fetch failed.");
    return null;
  }
}
