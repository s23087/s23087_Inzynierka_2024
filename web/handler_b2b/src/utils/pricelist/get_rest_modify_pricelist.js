"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getRestModifyPricelist(pricelistId) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/rest/pricelist/${pricelistId}/user/${userId}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return {
      maxQty: 0,
      items: [],
    };
  } catch {
    console.error("getRestModifyPricelist fetch failed.");
    return null;
  }
}
