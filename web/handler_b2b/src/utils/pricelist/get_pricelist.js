"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getPricelists(
  sort,
  totalL,
  totalR,
  status,
  currency,
  type,
  createdL,
  createdG,
  modifiedL,
  modifiedG,
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
  if (createdL) params.push(`createdL=${createdL}`);
  if (createdG) params.push(`createdG=${createdG}`);
  if (modifiedL) params.push(`modifiedL=${modifiedL}`);
  if (modifiedG) params.push(`modifiedG=${modifiedG}`);
  let url = `${process.env.API_DEST}/${dbName}/Offer/get/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch {
    console.error("getPricelists fetch failed.");
    return null;
  }
}
