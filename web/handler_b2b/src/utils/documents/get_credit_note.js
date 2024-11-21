"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

/**
 * Sends request to get credit notes. Can be filtered or sorted using sort and param arguments.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {boolean} isYourCredit Is type of credit note "Yours credit notes" boolean.
 * @param  {string} search Searched phrase.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {{dateL: string, dateG: string, qtyL: string, qtyG: string, totalL: string, totalG: string, recipient: string, currency: string, paymentStatus: string, status: string}} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<{user: string|undefined, creditNoteId: Number, invoiceNumber: string, date: string, qty: Number, total: Number, clientName: string, inSystem: boolean, isPaid: boolean}>>} Array of objects that contain credit note information. If connection was lost return null. If error occurred return empty array.
 */
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

    if (items.status === 400) {
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
