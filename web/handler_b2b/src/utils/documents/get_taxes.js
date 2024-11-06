"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get taxes lists.
 * @return {Promise<Array<{taxesId: Number, taxesValue: Number}>>} Array of objects that contain tax information. If connection was lost return null.
 */
export default async function getTaxes() {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/taxes`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getTaxes fetch failed.");
    return null;
  }
}
