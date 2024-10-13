"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function getSearchDeliveries(
  isOrg,
  isDeliveryToUser,
  search,
  sort,
  estimatedL,
  estimatedG,
  deliveredL,
  deliveredG,
  recipient,
  status,
  company,
  waybill,
) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let url = "";
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (estimatedL) params.push(`estimatedL=${estimatedL}`);
  if (estimatedG) params.push(`estimatedG=${estimatedG}`);
  if (deliveredL) params.push(`deliveredL=${deliveredL}`);
  if (deliveredG) params.push(`deliveredG=${deliveredG}`);
  if (recipient) params.push(`recipient=${recipient}`);
  if (status) params.push(`status=${status}`);
  if (company) params.push(`company=${company}`);
  if (waybill) params.push(`waybill=${waybill}`);
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Delivery/get/${isDeliveryToUser ? "user" : "client"}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Delivery/get/${isDeliveryToUser ? "user" : "client"}/${userId}?search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
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
