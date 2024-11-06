"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get necessary information for modifying organization.
 * @return {Promise<{id: Number, orgName: string, nip: Number|undefined, street: string, city: string, postalCode: string, countryId: Number, country: string}>}      Returns information about organization.
 */
export default async function getOrgModifyInfo() {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Settings/get/modify/rest/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return {};
  } catch {
    console.error("getOrgModifyInfo fetch failed.");
    return null;
  }
}
