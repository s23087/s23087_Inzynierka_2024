"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get rest information of chosen request needed to modify. If not found return object without properties. Other wise return object with properties receiverId and note.
 * @param {Number} requestId Request id.
 * @return {Promise<{receiverId: Number, note: string}>} Object that contain request information. If connection was lost return null.
 */
export default async function getRestModifyRequest(requestId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Requests/get/modify/rest/${requestId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return {};
  } catch {
    console.error("getRestModifyRequest fetch failed.");
    return null;
  }
}
