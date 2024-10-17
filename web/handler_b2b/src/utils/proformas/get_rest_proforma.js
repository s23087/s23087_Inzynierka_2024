"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestProforma(isYourProforma, proformaId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Proformas/get/${isYourProforma ? "yours" : "clients"}/rest/${proformaId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });
  
    if (info.ok) {
      return await info.json();
    }
  
    return [];
  } catch {
    console.error("getRestProforma fetch failed.")
    return null
  }
}
