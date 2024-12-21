"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get clients where organization name contains search string. Can be filtered or sorted using sort and country arguments. Will logout user if does not exist.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {string} search Searched phrase.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {string} country Filter clients by chosen country.
 * @return {Promise<Array<{clientId: Number, clientName: string, street: string, city: string, postal: string, nip: Number|undefined, country: string}>>}      Array of objects that contain invoice information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getSearchClients(isOrg, search, sort, country) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let url = "";
  const dbName = await getDbName();
  const userId = await getUserId();
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (country) params.push(`country=${country}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Client/get/org/${userId}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Client/get/${userId}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
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
    console.error("Get search client fetch failed.");
    return null;
  }
}
