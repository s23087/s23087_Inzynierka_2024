"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getRestInfo(currency, itemId, isOrg) {
  const dbName = await getDbName();
  let url = "";
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Warehouse/getRestOrgInfo?itemId=${itemId}&currency=${currency}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Warehouse/getRestInfo?itemId=${itemId}&userId=${userId}&currency=${currency}`;
  }
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.json();
  }

  return {};
}
