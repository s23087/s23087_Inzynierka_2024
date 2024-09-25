"use server";

import getDbName from "../auth/get_db_name";

export default async function getRestModifyCredit(creditId, isYourCredit) {
  const dbName = await getDbName();
  let url = `${process.env.API_DEST}/${dbName}/CreditNote/modify/rest/${isYourCredit}/${creditId}`;
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
}
