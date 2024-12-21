"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to modify organization. When data is unchanged the attribute in request will be null. If user do not exist server will logout them.
 * @param  {Number} orgId Organization id.
 * @param  {{ orgName: string, street: string, city: string, postalCode: string, statusId: Number}} prevState Object that contain information about previous state of chosen item.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function modifyClient(orgId, prevState, state, formData) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const userId = await getUserId();
  let nip = formData.get("nip");
  let credit = formData.get("credit");
  let messageError = validateData(orgId, formData);

  if (messageError.length > 6) {
    return {
      error: true,
      completed: true,
      message: messageError,
    };
  }

  let data = getData();

  const dbName = await getDbName();
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Client/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 404) {
      let text = await info.text();
      if (text === "Your account does not exists.") logout();
      return {
        ok: false,
        message: text,
      };
    }
    if (info.status === 500) {
      return {
        ok: false,
        message: "Server error.",
      };
    }

    if (info.ok) {
      return await setAvailabilityStatusesToClient(
        info,
        formData,
        prevState,
        orgId,
        dbName,
        userId,
      );
    }

    return {
      error: true,
      completed: true,
      message: "Critical error.",
    };
  } catch {
    console.error("Modify client fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
  /**
   * Organize information into object for fetch.
   * @return {object}
   */
  function getData() {
    return {
      orgId: orgId,
      orgName:
        formData.get("name") === prevState.orgName
          ? null
          : formData.get("name"),
      nip: nip === "" ? null : parseInt(nip),
      street:
        formData.get("street") === prevState.street
          ? null
          : formData.get("street"),
      city:
        formData.get("city") === prevState.city ? null : formData.get("city"),
      postalCode:
        formData.get("postal") === prevState.postalCode
          ? null
          : formData.get("postal"),
      creditLimit: credit ? parseInt(credit) : null,
      countryId: formData.get("country"),
    };
  }
}
/**
 * Sends request to set availability status to chosen organization. If user do not exist server will logout them.
 * @param  {Response} info Response of modify client fetch
 * @param  {FormData} formData
 * @param  {object} prevState Object that contain information about previous state of chosen item.
 * @param  {Number} orgId Organization id.
 * @param  {string} dbName Database name.
 * @param  {Number} userId Current user id.
 * @return {Promise<object>}      Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
async function setAvailabilityStatusesToClient(
  info,
  formData,
  prevState,
  orgId,
  dbName,
  userId,
) {
  let availability = formData.get("availability");

  if (parseInt(availability) === prevState.statusId) {
    return {
      error: false,
      completed: true,
      message: "Success! You have modified the client.",
    };
  }
  if (availability) {
    let statusData = {
      orgId: orgId,
      statusId: parseInt(formData.get("availability")),
    };

    const statusInfo = await fetch(
      `${process.env.API_DEST}/${dbName}/Client/setAvailabilityStatusesToClient/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(statusData),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 404) {
      let text = await info.text();
      if (text === "Your account does not exists.") logout();
      return {
        error: true,
        completed: true,
        message: text,
      };
    }

    if (info.status === 500) {
      return {
        error: true,
        completed: true,
        message: "Error: Changed organization, but not status. Server error.",
      };
    }

    if (statusInfo.ok) {
      return {
        error: false,
        completed: true,
        message: "Success! You have modified the client.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Error: Changed org spec, but not status. Please try again.",
      };
    }
  }

  return {
    error: false,
    completed: true,
    message: "Success! You have modified the client.",
  };
}

/**
 * Validate given form data.
 * @param  {string} orgId Organization id.
 * @param  {FormData} formData
 * @return {string} Return error message. If no error occurred retrun only "Error:"
 */
function validateData(orgId, formData) {
  let messageError = "Error:";

  if (!orgId) messageError += "\nOrg Id not found";
  if (
    !validators.lengthSmallerThen(formData.get("name"), 50) ||
    !validators.stringIsNotEmpty(formData.get("name"))
  )
    messageError += "\nOrg name is empty or length exceed 50 chars.";

  if (
    !validators.lengthSmallerThen(formData.get("street"), 200) ||
    !validators.stringIsNotEmpty(formData.get("street"))
  )
    messageError += "\nStreet is empty or length exceed 200 chars.";

  if (
    !validators.lengthSmallerThen(formData.get("city"), 200) ||
    !validators.stringIsNotEmpty(formData.get("city"))
  )
    messageError += "\nCity is empty or length exceed 200 chars.";

  if (
    !validators.lengthSmallerThen(formData.get("postal"), 200) ||
    !validators.stringIsNotEmpty(formData.get("postal"))
  )
    messageError += "\nPostal code is empty or length exceed 25 chars.";
  return messageError;
}
