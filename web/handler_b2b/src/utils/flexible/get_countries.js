"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get countries information.
 * @return {Promise<Array<{id: Number, countryName: string}>>} Object containing information about countries. If connection is lost returns null.
 */
export default async function getCountries() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Registration/countries`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getCountries fetch failed.");
    return null;
  }
}
