"use server";

import getDbName from "../auth/get_db_name";

export default async function setDeliveryStatus(deliveryId, statusName) {
  const dbName = await getDbName();
  let data = {
    statusName: statusName,
    deliveryId: deliveryId,
  };
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Delivery/status/change`,
    {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );
  return info.ok;
}
