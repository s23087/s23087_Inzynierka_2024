"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getRequests(
  isOrg,
  sort,
  dateL,
  dateG,
  type,
  status,
) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (dateL) params.push(`dateL=${dateL}`);
  if (dateG) params.push(`dateG=${dateG}`);
  if (type) params.push(`type=${type}`);
  if (status) params.push(`status=${status}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Requests/get/received/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Requests/get/created/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
  }
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
