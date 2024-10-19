"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestModifyProforma(proformaId) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/Proformas/get/rest/modify/${proformaId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return {
      status: false,
      paymentMethod: "Not found",
      note: "Not found",
    };
  } catch {
    console.error("getRestModifyProforma fetch failed.");
    return {
      status: false,
      paymentMethod: "Connection error.",
      note: "Connection error.",
    };
  }
}
