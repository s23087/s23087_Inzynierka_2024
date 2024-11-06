"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get invoice path.
 * @param {Number} invoiceId Invoice id.
 * @return {Promise<string>}      String that contain path information. If connection was lost or error occurred return null.
 */
export async function getInvoicePath(invoiceId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Invoices/get/path/${invoiceId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.text();
    }

    return null;
  } catch {
    console.error("Get invoice path fetch failed.");
    return null;
  }
}

/**
 * Sends request to get request path.
 * @param {Number} requestId Request id.
 * @return {Promise<string>}      String that contain path information. If connection was lost or error occurred return null.
 */
export async function getRequestPath(requestId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Requests/get/path/${requestId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.text();
    }

    return null;
  } catch {
    console.error("Get request path fetch failed.");
    return null;
  }
}

/**
 * Sends request to get credit note path.
 * @param {Number} creditId Credit note id.
 * @return {Promise<string>}      String that contain path information. If connection was lost or error occurred return null.
 */
export async function getCreditPath(creditId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/CreditNote/get/path/${creditId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.text();
    }

    return null;
  } catch {
    console.error("Get credit path fetch failed.");
    return null;
  }
}
