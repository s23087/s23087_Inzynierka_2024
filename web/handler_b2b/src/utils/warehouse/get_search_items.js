"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getSearchItems(
  currency,
  isOrg,
  search,
  sort,
  status,
  ean,
  qtyL,
  qtyG,
  priceL,
  priceG,
) {
  let url = "";
  const dbName = await getDbName();
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (qtyL) params.push(`qtyL=${qtyL}`);
  if (qtyG) params.push(`totalR=${qtyG}`);
  if (priceL) params.push(`qtyL=${qtyL}`);
  if (priceG) params.push(`totalR=${qtyG}`);
  if (status) params.push(`status=${status}`);
  if (ean) params.push(`ean=${ean}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Warehouse/items/${currency}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Warehouse/items/${currency}?userId=${userId}&search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  }
  try {
    const items = await fetch(url, {
      method: "GET",
    });

    if (items.ok) {
      return await items.json();
    }

    return [];
  } catch {
    console.error("getSearchItems fetch failed.")
    return null;
  }
}
