"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getProformas(
  isOrg,
  isYourProforma,
  sort,
  params
) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  let prepParams = getPrepParams(
    sort,
    params
  );
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Proformas/get/${isYourProforma ? "yours" : "clients"}${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Proformas/get/${isYourProforma ? "yours" : "clients"}/${userId}${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  }
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getProformas fetch failed.");
    return null;
  }
}

function getPrepParams(
  sort,
  params
) {
  let result = [];
  if (sort !== ".None") result.push(`sort=${sort}`);
  if (params.qtyL) result.push(`qtyL=${params.qtyL}`);
  if (params.qtyG) result.push(`qtyG=${params.qtyG}`);
  if (params.totalL) result.push(`totalL=${params.totalL}`);
  if (params.totalG) result.push(`totalG=${params.totalG}`);
  if (params.dateL) result.push(`dateL=${params.dateL}`);
  if (params.dateG) result.push(`dateG=${params.dateG}`);
  if (params.recipient) result.push(`recipient=${params.recipient}`);
  if (params.currency) result.push(`currency=${params.currency}`);
  return result;
}
