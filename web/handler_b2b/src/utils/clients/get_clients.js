"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

/**
 * Sends request to get clients. Can be filtered or sorted using sort and country arguments. Will logout user if does not exist.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {string} country Filter clients by chosen country.
 * @return {Promise<Array<{clientId: Number, clientName: string, street: string, city: string, postal: string, nip: Number|undefined, country: string}>>} Array of objects that contain invoice information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getClients(isOrg, sort, country) {
  let url = "";
  const userId = await getUserId();
  const dbName = await getDbName();
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (country) params.push(`country=${country}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Client/get/org/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Client/get/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
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
    console.error("Get client fetch failed.");
    return null;
  }
}
