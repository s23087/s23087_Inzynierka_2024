"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function getOutsideItems(
  role,
  sort,
  qtyL,
  qtyG,
  priceL,
  priceG,
  source,
  currency,
) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (qtyL) params.push(`qtyL=${qtyL}`);
  if (qtyG) params.push(`qtyG=${qtyG}`);
  if (priceL) params.push(`priceL=${priceL}`);
  if (priceG) params.push(`priceG=${priceG}`);
  if (source) params.push(`source=${source}`);
  if (currency) params.push(`currency=${currency}`);
  if (role === "Admin") {
    url = `${process.env.API_DEST}/${dbName}/OutsideItem/get/items${params.length > 0 ? "?" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/OutsideItem/get/items/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
  }
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
