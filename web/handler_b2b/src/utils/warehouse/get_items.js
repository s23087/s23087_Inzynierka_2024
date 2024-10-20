"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get items. Can be filtered or sorted using sort and param arguments.
 * @param  {[string]} currency Name of currency.
 * @param  {[boolean]} isOrg True if org view is activated, otherwise false.
 * @param  {[string]} sort Name of attribute that items will be sorted. Frist char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {[Object]} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {[Object]}      Array of object that contains item information. If connection was lost return null.
 */
export default async function getItems(currency, isOrg, sort, params) {
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
  } catch {
    console.error("getItems fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {[string]} sort Name of attribute that items will be sorted. Frist char indicates direction. D for descending and A for ascending.
 * @param  {[Object]} params Object that contains properties that items will be filtered by.
 * @return {[Object]}      Array of strings with prepared parameters.
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
