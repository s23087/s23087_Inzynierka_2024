"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";

export default async function createClient(state, formData) {
  let nip = formData.get("nip");
  let credit = formData.get("credit");
  let orgData = {
    orgName: formData.get("name"),
    nip: nip === "" ? null : parseInt(nip),
    street: formData.get("street"),
    city: formData.get("city"),
    postalCode: formData.get("postal"),
    creditLimit: credit === "" ? null : parseInt(credit),
    countryId: formData.get("country"),
  };
  let errorMessage = validateData(nip, credit, orgData);

  if (errorMessage.length > 6) {
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };
  }

  const dbName = await getDbName();
  const userId = await getUserId();
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Client/add/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(orgData),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 500) {
      return {
        error: true,
        completed: true,
        message: "Server error.",
      };
    }

    if (info.status === 404) {
      logout();
      return {
        error: true,
        completed: true,
        message: "Your account does not exists.",
      };
    }

    if (info.ok) {
      return await setAvailabilityStatusesToClient(formData, dbName, userId);
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error",
      };
    }
  } catch {
    console.error("Create client or set status fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error",
    };
  }
}
async function setAvailabilityStatusesToClient(formData, dbName, userId) {
  let orgId = await info.text();

  let statusData = {
    orgId: parseInt(orgId),
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

  if (statusInfo.status === 404) {
    let text = await statusInfo.text();
    if (text === "Your account does not exists.") logout();
    return {
      error: true,
      completed: true,
      message: text,
    };
  }

  if (statusInfo.status === 500) {
    return {
      error: true,
      completed: true,
      message: "Error: Created organization, but not status. Server error.",
    };
  }

  if (statusInfo.ok) {
    return {
      error: false,
      completed: true,
      message: "Success! You had added the new client",
    };
  }

  return {
    error: true,
    completed: true,
    message:
      "Org has been added, but something went wrong with Availability Status. Please change it in modify form.",
  };
}

function validateData(nip, credit, orgData) {
  let errorMessage = "Error:";
  if (isNipIncorrect(nip)) errorMessage += "\nNip must have only numbers.";

  if (isCreditIncorrect(credit))
    errorMessage += "\nCredit limit must have only numbers.";

  if (isOrgNameIncorrect(orgData.orgName))
    errorMessage += "\nOrg name is empty or length exceed 50 chars.";

  if (isStreetIncorrect(orgData.street))
    errorMessage += "\nStreet is empty or length exceed 200 chars.";

  if (isCityIncorrect(orgData.city))
    errorMessage += "\nCity is empty or length exceed 200 chars.";

  if (isPostalCodeIncorrect(orgData.postalCode))
    errorMessage += "\nPostal code is empty or length exceed 25 chars.";
  return errorMessage;
}

function isPostalCodeIncorrect(postalCode) {
  return (
    !validators.lengthSmallerThen(postalCode, 200) ||
    !validators.stringIsNotEmpty(postalCode)
  );
}

function isCityIncorrect(city) {
  return (
    !validators.lengthSmallerThen(city, 200) ||
    !validators.stringIsNotEmpty(city)
  );
}

function isStreetIncorrect(street) {
  return (
    !validators.lengthSmallerThen(street, 200) ||
    !validators.stringIsNotEmpty(street)
  );
}

function isOrgNameIncorrect(orgName) {
  return (
    !validators.lengthSmallerThen(orgName, 50) ||
    !validators.stringIsNotEmpty(orgName)
  );
}

function isCreditIncorrect(credit) {
  return (
    !validators.haveOnlyNumbers(credit) && validators.stringIsNotEmpty(credit)
  );
}

function isNipIncorrect(nip) {
  return !validators.haveOnlyNumbers(nip) && validators.stringIsNotEmpty(nip);
}
