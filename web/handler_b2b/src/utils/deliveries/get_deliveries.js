"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function getDeliveries(
  isOrg,
  isDeliveryToUser,
  sort,
  params,
) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  let prepParams = getParams(sort, params);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Delivery/get/${isDeliveryToUser ? "user" : "client"}${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Delivery/get/${isDeliveryToUser ? "user" : "client"}/${userId}${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  }
  try {
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
  } catch {
    console.error("getDeliveries fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {[string]} sort Name of attribute that items will be sorted. Frist char indicates direction. D for descending and A for ascending.
 * @param  {[Object]} params Object that contains properties that items will be filtered by.
 * @return {[Object]}      Array of strings with prepared parameters.
 */
function getParams(sort, params) {
  let result = [];
  if (sort !== ".None") result.push(`sort=${sort}`);
  if (params.estimatedL) result.push(`estimatedL=${params.estimatedL}`);
  if (params.estimatedG) result.push(`estimatedG=${params.estimatedG}`);
  if (params.deliveredL) result.push(`deliveredL=${params.deliveredL}`);
  if (params.deliveredG) result.push(`deliveredG=${params.deliveredG}`);
  if (params.recipient) result.push(`recipient=${params.recipient}`);
  if (params.status) result.push(`status=${params.status}`);
  if (params.company) result.push(`company=${params.company}`);
  if (params.waybill) result.push(`waybill=${params.waybill}`);
  return result;
}
