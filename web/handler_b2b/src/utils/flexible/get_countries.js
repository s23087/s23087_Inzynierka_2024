"use server";

import getDbName from "../auth/get_db_name";

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
