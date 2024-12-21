"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get list of organizations.
 * @return {Promise<Array<{userOrgId: Number, orgName: string, restOrgs: Array<{orgId: Number, orgName: string}>}>>} Array of objects that contain organization information. If connection was lost return null.
 */
export default async function getOrgsList() {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/orgs/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return {
      restOrgs: [],
    };
  } catch (error) {
    console.error(error);
    console.error("getOrgsList fetch failed.");
    return null;
  }
}
