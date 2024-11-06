"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

/**
 * Sends request to create delivery.
 * @param  {Array<string>} waybills Array that contain waybills.
 * @param  {boolean} isDeliveryToUser If delivery is of type "Deliveries to user" it value is true, otherwise false.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function createDelivery(
  waybills,
  isDeliveryToUser,
  state,
  formData,
) {
  let errorMessage = "Error:";
  let proforma = formData.get("proforma");
  let date = formData.get("date");
  let company = formData.get("company");
  let note = formData.get("note");
  if (!proforma) errorMessage += "\nProforma must not be empty.";
  if (!date) errorMessage += "\nDate must not be empty.";
  if (!company) errorMessage += "\nCompany must not be empty.";
  if (waybills.length === 0)
    errorMessage += "\nMust have at least one waybill.";

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  const userId = await getUserId();
  const dbName = await getDbName();

  let deliveryData = {
    userId: userId,
    isDeliveryToUser: isDeliveryToUser,
    estimatedDeliveryDate: date,
    proformaId: parseInt(proforma),
    companyId: parseInt(company),
    waybills: waybills,
    note: note === "" ? null : note,
  };

  try {
    const info = await fetch(`${process.env.API_DEST}/${dbName}/Delivery/add`, {
      method: "POST",
      body: JSON.stringify(deliveryData),
      headers: {
        "Content-Type": "application/json",
      },
    });
    if (info.status === 404) {
      logout();
      return {
        error: true,
        completed: true,
        message: "Your user profile does not exists.",
      };
    }
    if (info.status === 400) {
      return {
        error: true,
        completed: true,
        message: await info.text(),
      };
    }
    if (info.status === 500) {
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
        message: "Success! You had created delivery.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error.",
      };
    }
  } catch {
    console.error("createDelivery fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
