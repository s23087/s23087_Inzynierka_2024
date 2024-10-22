"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get pricelists. Can be filtered or sorted using sort and param arguments.
 * @param  {string} sort Name of attribute that items will be sorted. Frist char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {object} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<Object>>}      Array of objects that contain pricelist information. If connection was lost return null.
 */
export default async function getPricelists(sort, params) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let prepParams = getPrepParams(sort, params);
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/${userId}${prepParams.length > 0 ? "?" : ""}${prepParams.join("&")}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getPricelists fetch failed.");
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
  if (params.totalL) result.push(`totalL=${params.totalL}`);
  if (params.totalR) result.push(`totalR=${params.totalR}`);
  if (params.status) result.push(`status=${params.status}`);
  if (params.currency) result.push(`currency=${params.currency}`);
  if (params.type) result.push(`type=${params.type}`);
  if (params.createdL) result.push(`createdL=${params.createdL}`);
  if (params.createdG) result.push(`createdG=${params.createdG}`);
  if (params.modifiedL) result.push(`modifiedL=${params.modifiedL}`);
  if (params.modifiedG) result.push(`modifiedG=${params.modifiedG}`);
  return result;
}
