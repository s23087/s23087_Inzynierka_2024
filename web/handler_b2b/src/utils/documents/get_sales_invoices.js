"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

/**
 * Prepares params for joining to url.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending.
 * @param  {{dateL: string, dateG: string, dueL: string, dueG: string, qtyL: string, qtyG: string, totalL: string, totalG: string, recipient: string, currency: string, paymentStatus: string, status: string}} params Object that contains properties that items will be filtered by.
 * @return {Array<string>}      Array of strings with prepared parameters.
 */
function getPrepParams(sort, params) {
  let prepParams = [];
  if (sort !== ".None") prepParams.push(`sort=${sort}`);
  if (params.dateL) prepParams.push(`dateL=${params.dateL}`);
  if (params.dateG) prepParams.push(`dateG=${params.dateG}`);
  if (params.dueL) prepParams.push(`dueL=${params.dueL}`);
  if (params.dueG) prepParams.push(`dueG=${params.dueG}`);
  if (params.qtyL) prepParams.push(`qtyL=${params.qtyL}`);
  if (params.qtyG) prepParams.push(`qtyG=${params.qtyG}`);
  if (params.totalL) prepParams.push(`totalL=${params.totalL}`);
  if (params.totalG) prepParams.push(`totalG=${params.totalG}`);
  if (params.recipient) prepParams.push(`recipient=${params.recipient}`);
  if (params.currency) prepParams.push(`currency=${params.currency}`);
  if (params.paymentStatus)
    prepParams.push(`paymentStatus=${params.paymentStatus}`);
  if (params.status) prepParams.push(`status=${params.status}`);
  return prepParams;
}

/**
 * Sends request to get sales invoices. Can be filtered or sorted using sort and param arguments.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {string} search Searched phrase.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {{dateL: string, dateG: string, dueL: string, dueG: string, qtyL: string, qtyG: string, totalL: string, totalG: string, recipient: string, currency: string, paymentStatus: string, status: string}} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<{users: Array<string>|undefined, invoiceId: Number, invoiceNumber: string, clientName: string, invoiceDate: string, dueDate: string, paymentStatus: string, inSystem: boolean, qty: Number, price: Number}>>}      Array of objects that contain invoice information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getSalesInvoices(isOrg, sort, params) {
  let url = "";
  let prepParams = getPrepParams(sort, params);
  const dbName = await getDbName();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/sales/org${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/sales/${userId}${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  }
  try {
    const items = await fetch(url, {
      method: "GET",
    });

    if (items.status === 404) {
      logout();
    }

    if (items.ok) {
      return await items.json();
    }

    return [];
  } catch (error) {
    console.error("getSalesInvoices fetch failed.");
    return null;
  }
}
