"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";

export default async function modifyClient(orgId, prevState, state, formData) {
  const userId = await getUserId();
  let nip = formData.get("nip");
  let credit = formData.get("credit");
  let messageError = validateData(orgId, formData);

  if (messageError.length > 7) {
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

async function setAvailabilityStatusesToClient(
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

function validateData(orgId, formData) {
  let messageError = "Errors:";

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
