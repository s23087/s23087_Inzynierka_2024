"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to modify delivery. When data is unchanged the attribute in request will be null.
 * @param  {Number} deliveryId Delivery id.
 * @param  {Array<string>} waybills Array that contain waybills.
 * @param  {boolean} isDeliveryToUser If delivery is of type "Deliveries to user" it value is true, otherwise false.
 * @param  {{estimated: string, isWaybillModified: boolean}} prevState Object that contain information about previous state of chosen invoice.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function updateDelivery(
  deliveryId,
  waybills,
  isDeliveryToUser,
  prevState,
  state,
  formData,
) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let estimated = formData.get("estimated");
  let company = formData.get("company");

  let message = "Error:";
  if (!estimated) message += "\nName is empty or exceed required length";
  if (!company) message += "\nDescription is empty or exceed required length";

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
  try {
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
  } catch {
    console.error("updateDelivery fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
