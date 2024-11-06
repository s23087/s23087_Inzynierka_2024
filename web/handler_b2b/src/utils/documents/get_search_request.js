"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get user requests where title contains search string. Can be filtered or sorted using sort and parameters.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {string} search Searched phrase.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {string} dateL Request date lower or equal then chosen date parameter.
 * @param  {string} dateG Request date greater or equal then chosen date parameter.
 * @param  {string} type Filter request by chosen type.\
 * @param  {Number} status Filter request by chosen status.
 * @return {Promise<Array<{id: Number, username: string, status: string, objectType: string, creationDate: string, title: string}>>}      Array of objects that contain request information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getSearchRequests(
  isOrg,
  search,
  sort,
  dateL,
  dateG,
  type,
  status,
) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (dateL) params.push(`dateL=${dateL}`);
  if (dateG) params.push(`dateG=${dateG}`);
  if (type) params.push(`type=${type}`);
  if (status) params.push(`status=${status}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Requests/get/received/${userId}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Requests/get/created/${userId}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  }
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getSearchRequests fetch failed.");
    return null;
  }
}
