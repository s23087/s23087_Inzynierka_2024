"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get outside items. Can be filtered or sorted using sort and param arguments.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {{qtyL: string, qtyG: string, priceL: string, priceG: string, source: string, currency: string}} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<{users: Array<string>|undefined, partnumber: string, itemId: Number, orgId: Number, orgName: string, price: Number, qty: Number, currency: string}>>}      Array of objects that contain outside item information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getOutsideItems(sort, params) {
  const dbName = await getDbName();
  let url = "";
  let prepParams = getPrepParams(sort, params);
  url = `${process.env.API_DEST}/${dbName}/OutsideItem/get/items${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getOutsideItems fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending.
 * @param  {{qtyL: string, qtyG: string, priceL: string, priceG: string, source: string, currency: string}} params Object that contains properties that items will be filtered by.
 * @return {Array<string>}      Array of strings with prepared parameters.
 */
function getPrepParams(sort, params) {
  let result = [];
  if (sort !== ".None") result.push(`sort=${sort}`);
  if (params.qtyL) result.push(`qtyL=${params.qtyL}`);
  if (params.qtyG) result.push(`qtyG=${params.qtyG}`);
  if (params.priceL) result.push(`priceL=${params.priceL}`);
  if (params.priceG) result.push(`priceG=${params.priceG}`);
  if (params.source) result.push(`source=${params.source}`);
  if (params.currency) result.push(`currency=${params.currency}`);
  return result;
}
