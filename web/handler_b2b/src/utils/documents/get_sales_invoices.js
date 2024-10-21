"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

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

    if (items.ok) {
      return await items.json();
    }

    return [];
  } catch (error) {
    console.error("getSalesInvoices fetch failed.");
    return null;
  }
}
