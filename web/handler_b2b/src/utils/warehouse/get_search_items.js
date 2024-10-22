"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get items that part number or name contains search string. Can be filtered or sorted using sort and param arguments.
 * @param  {string} currency Name of currency.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {string} search Searched phrase.
 * @param  {string} sort Name of attribute that items will be sorted. Frist char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {object} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<Object>>}      Array of objects that contain item information. If connection was lost return null.
 */
export default async function getSearchItems(
  currency,
  isOrg,
  search,
  sort,
  params,
) {
  let url = "";
  const dbName = await getDbName();
  let prepParams = getPrepParams(sort, params);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Warehouse/items/${currency}?search=${search}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Warehouse/items/${currency}?userId=${userId}&search=${search}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
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
    console.error("getSearchItems fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {string} sort Name of attribute that items will be sorted. Frist char indicates direction. D for descending and A for ascending.
 * @param  {object} params Object that contains properties that items will be filtered by.
 * @return {Array<string>}      Array of strings with prepared parameters.
 */
function getPrepParams(sort, params) {
  let result = [];
  if (sort !== ".None") result.push(`sort=${sort}`);
  if (params.qtyL) result.push(`qtyL=${params.qtyL}`);
  if (params.qtyG) result.push(`totalR=${params.qtyG}`);
  if (params.priceL) result.push(`qtyL=${params.qtyL}`);
  if (params.priceG) result.push(`totalR=${params.qtyG}`);
  if (params.status) result.push(`status=${params.status}`);
  if (params.ean) result.push(`ean=${params.ean}`);
  return result;
}
