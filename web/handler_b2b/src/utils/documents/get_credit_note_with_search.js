"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get credit notes where invoice number or organization contains search string. Can be filtered or sorted using sort and param arguments.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {boolean} isYourCredit Is type of credit note "Yours credit notes" boolean.
 * @param  {string} search Searched phrase.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {{dateL: string, dateG: string, qtyL: string, qtyG: string, totalL: string, totalG: string, recipient: string, currency: string, paymentStatus: string, status: string}} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<{user: string|undefined, creditNoteId: Number, invoiceNumber: string, date: string, qty: Number, total: Number, clientName: string, inSystem: boolean, isPaid: boolean}>>} Array of objects that contain credit note information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getSearchCreditNotes(
  isOrg,
  isYourCredit,
  search,
  sort,
  params,
) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let url = "";
  let prepParams = getPrepParams(sort, params);
  const dbName = await getDbName();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/CreditNote/get/${isYourCredit}?search=${search}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/CreditNote/get/${isYourCredit}/${userId}?search=${search}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
  }
  try {
    const items = await fetch(url, {
      method: "GET",
    });

    if (items.status === 404) {
      logout();
      return [];
    }

    if (items.status === 400) {
      return [];
    }

    if (items.ok) {
      return await items.json();
    }

    return [];
  } catch {
    console.error("getSearchCreditNotes fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending.
 * @param  {{dateL: string, dateG: string, qtyL: string, qtyG: string, totalL: string, totalG: string, recipient: string, currency: string, paymentStatus: string, status: string}} params Object that contains properties that items will be filtered by.
 * @return {Array<string>} Array of strings with prepared parameters.
 */
function getPrepParams(sort, params) {
  let result = [];
  if (sort !== ".None") result.push(`sort=${sort}`);
  if (params.dateL) result.push(`dateL=${params.dateL}`);
  if (params.dateG) result.push(`dateG=${params.dateG}`);
  if (params.qtyL) result.push(`qtyL=${params.qtyL}`);
  if (params.qtyG) result.push(`qtyG=${params.qtyG}`);
  if (params.totalL) result.push(`totalL=${params.totalL}`);
  if (params.totalG) result.push(`totalG=${params.totalG}`);
  if (params.recipient) result.push(`recipient=${params.recipient}`);
  if (params.currency) result.push(`currency=${params.currency}`);
  if (params.paymentStatus)
    result.push(`paymentStatus=${params.paymentStatus}`);
  if (params.status) result.push(`status=${params.status}`);
  return result;
}
