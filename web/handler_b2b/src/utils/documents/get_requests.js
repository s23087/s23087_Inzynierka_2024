"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get user requests. Can be filtered or sorted using sort and parameters.
 * @param  {boolean} isOrg True if org view is activated, otherwise false.
 * @param  {string} sort Name of attribute that items will be sorted. First char indicates direction. D for descending and A for ascending. This param cannot be omitted.
 * @param  {string} dateL Request date lower or equal then chosen date parameter.
 * @param  {string} dateG Request date greater or equal then chosen date parameter.
 * @param  {string} type Filter request by chosen type.\
 * @param  {Number} status Filter request by chosen status.
 * @return {Promise<Array<{id: Number, username: string, status: string, objectType: string, creationDate: string, title: string}>>}      Array of objects that contain request information. If connection was lost return null.
 */
export default async function getRequests(
  isOrg,
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
    url = `${process.env.API_DEST}/${dbName}/Requests/get/received/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Requests/get/created/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
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
    console.error("getRequests fetch failed.");
    return null;
  }
}
