"use server";

import getDbName from "../auth/get_db_name";

/**
 * Sends request to get rest information of chosen proforma.
 * @param  {boolean} isYourProforma Is proforma type "Yours proformas".
 * @param  {Number} proformaId Proforma id.
 * @return {Promise<object>}      Return object containing proforma object information. If connection is lost return null.
 */
export default async function getRestProforma(isYourProforma, proformaId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Proformas/get/${isYourProforma ? "yours" : "clients"}/rest/${proformaId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getRestProforma fetch failed.");
    return null;
  }
}
