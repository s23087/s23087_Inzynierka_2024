"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get items. Can be filtered or sorted using sort and param arguments.
 * @param  {string} currency Name of currency.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {{qtyL: string, qtyG: string, priceL: string, priceG: string, status: string, ean: string}} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<{users: Array<string>, itemId: Number, itemName: string, partNumber: string, statusName: string, eans: Array<string>, qty: Number, purchasePrice: Number, sources: Array<string>}>>}      Array of objects that contain item information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getItems(currency, isOrg, sort, params) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let url = "";
  const dbName = await getDbName();
  let prepParams = getPrepParams(sort, params);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Warehouse/get/${currency}${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Warehouse/get/${currency}?userId=${userId}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
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
    console.error(error);
    console.error("getItems fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending.
 * @param  {{qtyL: string, qtyG: string, priceL: string, priceG: string, status: string, ean: string}} params Object that contains properties that items will be filtered by.
 * @return {Array<string>}      Array of strings with prepared parameters.
 */
function getPrepParams(sort, params) {
  let result = [];
  if (sort !== ".None") result.push(`sort=${sort}`);
  if (params.qtyL) result.push(`qtyL=${params.qtyL}`);
  if (params.qtyG) result.push(`qtyG=${params.qtyG}`);
  if (params.priceL) result.push(`priceL=${params.priceL}`);
  if (params.priceG) result.push(`priceG=${params.priceG}`);
  if (params.status) result.push(`status=${params.status}`);
  if (params.ean) result.push(`ean=${params.ean}`);
  return result;
}
