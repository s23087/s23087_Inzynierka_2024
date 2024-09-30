"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function getDeliveries(isOrg, isDeliveryToUser) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Delivery/get/${isDeliveryToUser ? "user" : "client"}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Delivery/get/${isDeliveryToUser ? "user" : "client"}/${userId}`;
  }
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
