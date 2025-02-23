"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get proformas that proforma number contains search string. Can be filtered or sorted using sort and param arguments.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {boolean} isYourProforma Is proforma type "Yours proformas".
 * @param  {string} search Searched phrase.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {{qtyL: string, qtyG: string, totalL: string, totalG: string, dateL: string, dateG: string, recipient: string, currency: string}} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<{user: string|undefined, proformaId: Number, date: string, transport: Number, qty: Number, total: Number, currencyName: string, clientName: string}>>} Array of objects that contain item information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getSearchProformas(
  isOrg,
  isYourProforma,
  search,
  sort,
  params,
) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  let prepParams = getPrepParams(sort, params);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Proformas/get/${isYourProforma ? "yours" : "clients"}?search=${search}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Proformas/get/${isYourProforma ? "yours" : "clients"}/${userId}?search=${search}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
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
    console.error("getSearchProformas fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending.
 * @param  {{qtyL: string, qtyG: string, totalL: string, totalG: string, dateL: string, dateG: string, recipient: string, currency: string}} params Object that contains properties that items will be filtered by.
 * @return {Array<string>}      Array of strings with prepared parameters.
 */
function getPrepParams(sort, params) {
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
