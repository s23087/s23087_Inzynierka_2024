"use server";

import getDbName from "../auth/get_db_name";

export default async function getDeliveryCompany() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Delivery/get/delivery_companies`;
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.json();
  }

  return [];
}
