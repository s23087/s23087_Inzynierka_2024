"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get list of organizations.
 * @return {Promise<Array<{userOrgId: Number, orgName: string, restOrgs: Array<{orgId: Number, orgName: string}>}>>} Array of objects that contain organization information. If connection was lost return null.
 */
export default async function getOrgsList() {
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
  } catch {
    console.error("getOrgsList fetch failed.");
    return null;
  }
}
