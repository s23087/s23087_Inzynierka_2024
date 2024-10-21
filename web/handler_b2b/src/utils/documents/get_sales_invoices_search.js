"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getSearchSalesInvoices(
  isOrg,
  search,
  sort,
  params,
) {
  let url = "";
  let prepParams = getPreparedParams(sort, params);
  const dbName = await getDbName();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/sales/org?search=${search}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/sales/${userId}?search=${search}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
  }
  try {
    const items = await fetch(url, {
      method: "GET",
    });

    if (items.ok) {
      return await items.json();
    }

    return [];
  } catch {
    console.error("getSearchSalesInvoices fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {[string]} sort Name of attribute that items will be sorted. Frist char indicates direction. D for descending and A for ascending.
 * @param  {[Object]} params Object that contains properties that items will be filtered by.
 * @return {[Object]}      Array of strings with prepared parameters.
 */
function getPreparedParams(sort, params) {
  let result = [];
  if (sort !== ".None") result.push(`sort=${sort}`);
  if (params.dateL) result.push(`dateL=${params.dateL}`);
  if (params.dateG) result.push(`dateG=${params.dateG}`);
  if (params.dueL) result.push(`dueL=${params.dueL}`);
  if (params.dueG) result.push(`dueG=${params.dueG}`);
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
