"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getSearchProformas(
  isOrg,
  isYourProforma,
  search,
  sort,
  qtyL,
  qtyG,
  totalL,
  totalG,
  dateL,
  dateG,
  recipient,
  currency,
) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (qtyL) params.push(`qtyL=${qtyL}`);
  if (qtyG) params.push(`qtyG=${qtyG}`);
  if (totalL) params.push(`totalL=${totalL}`);
  if (totalG) params.push(`totalG=${totalG}`);
  if (dateL) params.push(`dateL=${dateL}`);
  if (dateG) params.push(`dateG=${dateG}`);
  if (recipient) params.push(`recipient=${recipient}`);
  if (currency) params.push(`currency=${currency}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Proformas/get/${isYourProforma ? "yours" : "clients"}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Proformas/get/${isYourProforma ? "yours" : "clients"}/${userId}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  }
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return [];
  } catch (error) {
    console.log(error);
    return null;
  }
}
