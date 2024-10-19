"use server";

import getDbName from "../auth/get_db_name";

export default async function getProformaPath(proformaId) {
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
