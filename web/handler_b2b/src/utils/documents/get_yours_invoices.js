"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";

export default async function getYoursInvoices(
  isOrg,
  sort,
  dateL,
  dateG,
  dueL,
  dueG,
  qtyL,
  qtyG,
  totalL,
  totalG,
  recipient,
  currency,
  paymentStatus,
  status,
) {
  let url = "";
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (dateL) params.push(`dateL=${dateL}`);
  if (dateG) params.push(`dateG=${dateG}`);
  if (dueL) params.push(`dueL=${dueL}`);
  if (dueG) params.push(`dueG=${dueG}`);
  if (qtyL) params.push(`qtyL=${qtyL}`);
  if (qtyG) params.push(`qtyG=${qtyG}`);
  if (totalL) params.push(`totalL=${totalL}`);
  if (totalG) params.push(`totalG=${totalG}`);
  if (recipient) params.push(`recipient=${recipient}`);
  if (currency) params.push(`currency=${currency}`);
  if (paymentStatus) params.push(`paymentStatus=${paymentStatus}`);
  if (status) params.push(`status=${status}`);
  const dbName = await getDbName();
  if (isOrg) {
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/purchase/org${params.length > 0 ? "?" : ""}${params.join("&")}`;
  } else {
    const userId = await getUserId();
    url = `${process.env.API_DEST}/${dbName}/Invoices/get/purchase/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
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
    return null;
  }
}
