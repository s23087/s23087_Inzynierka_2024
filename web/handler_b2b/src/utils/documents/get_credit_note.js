"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function getCreditNotes(
  isOrg,
  isYourCredit,
  sort,
  params,
) {
  let url = "";
  let prepParams = getPrepParams(sort, params);
  const dbName = await getDbName();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/CreditNote/get/${isYourCredit}${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/CreditNote/get/${isYourCredit}/${userId}${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  }
  try {
    const items = await fetch(url, {
      method: "GET",
    });

    if (items.status === 404) {
      logout();
      return [];
    }

    if (items.ok) {
      return await items.json();
    }

    return [];
  } catch {
    console.error("getCreditNotes fetch failed.");
    return null;
  }
}

function getPrepParams(sort, params) {
  let result = [];
  if (sort !== ".None") result.push(`sort=${sort}`);
  if (params.dateL) result.push(`dateL=${params.dateL}`);
  if (params.dateG) result.push(`dateG=${params.dateG}`);
  if (params.qtyL) result.push(`qtyL=${params.qtyL}`);
  if (params.qtyG) result.push(`qtyG=${params.tyG}`);
  if (params.totalL) result.push(`totalL=${params.totalL}`);
  if (params.totalG) result.push(`totalG=${params.totalG}`);
  if (params.recipient) result.push(`recipient=${params.recipient}`);
  if (params.currency) result.push(`currency=${params.currency}`);
  if (params.paymentStatus)
    result.push(`paymentStatus=${params.paymentStatus}`);
  if (params.status) result.push(`status=${params.status}`);
  return result;
}
