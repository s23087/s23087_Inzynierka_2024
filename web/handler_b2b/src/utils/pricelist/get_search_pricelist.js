"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getSearchPricelists(
  search,
  sort,
  totalL,
  totalR,
  status,
  currency,
  type,
) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (totalL) params.push(`totalL=${totalL}`);
  if (totalR) params.push(`totalR=${totalR}`);
  if (status) params.push(`status=${status}`);
  if (currency) params.push(`currency=${currency}`);
  if (type) params.push(`type=${type}`);
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/${userId}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  console.log(url);
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    return null;
  }
}
