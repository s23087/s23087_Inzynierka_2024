"use server";

import getDbName from "../auth/get_db_name";

export default async function getOfferStatuses() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/statues`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    return [];
  }
}
