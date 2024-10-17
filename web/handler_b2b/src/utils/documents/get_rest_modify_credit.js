"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestModifyCredit(creditId, isYourCredit) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/CreditNote/get/rest/modify/${isYourCredit}/${creditId}`;
  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.ok) {
      return await data.json();
    }

    return {
      creditNumber: "Not found",
      OrgName: "Not found",
      Note: "Not found",
    };
  } catch {
    console.error("getRestModifyCredit fetch failed.");
    return {
      creditNumber: "Connection error.",
      OrgName: "Connection error.",
      Note: "Connection error.",
    };
  }
}
