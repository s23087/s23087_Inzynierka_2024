"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function getProformaListWithoutDelivery(isDeliveryToUser) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Delivery/get/${isDeliveryToUser ? "user" : "client"}/proformas/${userId}`;
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.status === 404) {
    logout();
    return [];
  }

  if (info.ok) {
    return await info.json();
  }

  return [];
}
