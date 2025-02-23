"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get pricelists that name contains search string. Can be filtered or sorted using sort and param arguments.
 * @param  {string} search Searched phrase.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {{totalL: string, totalR: string, status: string, currency: string, type: string, createdL: string, createdG: string, modifiedL: string, modifiedG: string}} params Object that contains properties that items will be filtered by. This param cannot be omitted.
 * @return {Promise<Array<{pricelistId: Number, created: string, status: string, name: string, totalItems: Number, path: string, currency: string, modified: string}>>} Array of objects that contain pricelist information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getSearchPricelists(search, sort, params) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let prepParams = getPrepParams(sort, params);
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/${userId}?search=${search}${prepParams.length > 0 ? "&" : ""}${prepParams.join("&")}`;
  console.log(url);
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getSearchPricelists fetch failed.");
    return null;
  }
}

/**
 * Prepares params for joining to url.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending.
 * @param  {{totalL: string, totalR: string, status: string, currency: string, type: string, createdL: string, createdG: string, modifiedL: string, modifiedG: string}} params Object that contains properties that items will be filtered by.
 * @return {Array<string>} Array of strings with prepared parameters.
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
