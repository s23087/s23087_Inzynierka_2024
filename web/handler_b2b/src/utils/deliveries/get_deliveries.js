"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get deliveries. Can be filtered or sorted using sort and param arguments.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {boolean} isDeliveryToUser Is delivery type "Deliveries to user" boolean.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {{estimatedL: string, estimatedG: string, deliveredL: string, deliveredG: string, recipient: string, status: string, company: string, waybill: string}} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<{user: string, deliveryId: Number, status: string, waybill: Array<string>, deliveryCompany: string, estimated: string, proforma: string, clientName: string, delivered: string}>>} Array of objects that contain delivery information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getDeliveries(
  isOrg,
  isDeliveryToUser,
  sort,
  params,
) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
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

    if (info.status === 400) {
      return [];
    }

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch (error) {
    console.error(error);
    console.error("getDeliveries fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending.
 * @param  {{estimatedL: string, estimatedG: string, deliveredL: string, deliveredG: string, recipient: string, status: string, company: string, waybill: string}} params Object that contains properties that items will be filtered by.
 * @return {Array<string>}      Array of strings with prepared parameters.
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
