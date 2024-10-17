"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestClientInfo(orgId) {
  const dbName = await getDbName();
  try {
    let url = `${process.env.API_DEST}/${dbName}/Client/get/rest/${orgId}`;
    const info = await fetch(url, {
      method: "GET",
    });

    if (items.status === 404) {
      return {
        creditLimit: null,
        availability: "This object do not exist",
        daysForRealization: null,
      };
    }

    if (info.ok) {
      return await info.json();
    }

    return {};
  } catch {
    console.error("Get rest client info fetch failed.");
    return null;
  }
}
