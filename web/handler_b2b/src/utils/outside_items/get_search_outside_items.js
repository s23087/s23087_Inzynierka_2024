"use server";

import getDbName from "../auth/get_db_name";
import logout from "../auth/logout";

export default async function getSearchOutsideItems(
  search,
  sort,
  qtyL,
  qtyG,
  priceL,
  priceG,
  source,
  currency,
) {
  const dbName = await getDbName();
  let url = "";
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (qtyL) params.push(`qtyL=${qtyL}`);
  if (qtyG) params.push(`qtyG=${qtyG}`);
  if (priceL) params.push(`priceL=${priceL}`);
  if (priceG) params.push(`priceG=${priceG}`);
  if (source) params.push(`source=${source}`);
  if (currency) params.push(`currency=${currency}`);
  url = `${process.env.API_DEST}/${dbName}/OutsideItem/get/items?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });
    if (info.status === 404) {
      logout();
      return [];
    }

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    return null;
  }
}
