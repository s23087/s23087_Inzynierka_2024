"use server";

import getDbName from "../auth/get_db_name";

export default async function getDeliveryStatuses() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Delivery/get/delivery_statuses`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getDeliveryStatuses fetch failed.");
    return null;
  }
}
