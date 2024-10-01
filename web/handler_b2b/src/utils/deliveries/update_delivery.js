"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function updateDelivery(
  deliveryId,
  waybills,
  isDeliveryToUser,
  prevState,
  state,
  formData,
) {
  let estimated = formData.get("estimated");
  let company = formData.get("company");

  let message = "Error:";
  if (!estimated) message += "\nName is empty or exceed required lenght";
  if (!company) message += "\nDescription is empty or exceed required lenght";

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }
  let data = {
    deliveryId: deliveryId,
    estimated: prevState.estimated === estimated ? null : estimated,
    companyId: parseInt(company) === -1 ? null : parseInt(company),
    waybills: prevState.isWaybillModified ? waybills : null,
  };

  const userId = await getUserId();
  const dbName = await getDbName();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Delivery/modify/${isDeliveryToUser}/${userId}`,
    {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  if (info.status == 404) {
    let text = await info.text();
    if (text === "User not found.") {
      logout();
      return {
        error: true,
        completed: true,
        message: "Your account does not exist.",
      };
    }
    return {
      error: true,
      completed: true,
      message: text,
    };
  }

  if (info.status == 500) {
    return {
      error: true,
      completed: true,
      message: "Server error.",
    };
  }

  if (info.ok) {
    return {
      error: false,
      completed: true,
      message: "Success! Delivery has been modified.",
    };
  } else {
    return {
      error: true,
      completed: true,
      message: "Critical error",
    };
  }
}
