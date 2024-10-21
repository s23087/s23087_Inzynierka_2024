"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

/**
 * Sends request to get rest information of chosen item from invoices and outside items data.
 * @param  {[String]} currency Name of currency.
 * @param  {[Number]} itemId Item id.
 * @param  {[boolean]} isOrg True if org view is activated, otherwise false.
 * @return {[Object]}      Object containing rest item information. If connection was lost return null.
 */
export default async function getRestInfo(currency, itemId, isOrg) {
  const dbName = await getDbName();
  let url = "";
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Warehouse/get/rest/org/${itemId}/${currency}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Warehouse/get/rest/${itemId}/${currency}/user/${userId}`;
  }
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return {};
  } catch {
    console.error("getRestInfo fetch failed.");
    return null;
  }
}
