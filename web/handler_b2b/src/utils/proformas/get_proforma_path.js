"use server";

import getDbName from "../auth/get_db_name";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get proforma path.
 * @param  {Number} proformaId Proforma id.
 * @return {Promise<string>}      Return string containing path. If error occurred return null.
 */
export default async function getProformaPath(proformaId) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Proformas/get/path/${proformaId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.text();
    }

    return null;
  } catch {
    console.error("getProformaPath fetch failed.");
    return null;
  }
}
