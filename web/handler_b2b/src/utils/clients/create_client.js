"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";

export default async function createClient(state, formData) {
  let nip = formData.get("nip");
  let credit = formData.get("credit");
  if (!validators.haveOnlyNumbers(nip) && validators.stringIsNotEmpty(nip))
    return {
      error: true,
      completed: true,
      message: "Error: Nip must have only numbers.",
    };
  if (
    !validators.haveOnlyNumbers(credit) &&
    validators.stringIsNotEmpty(credit)
  )
    return {
      error: true,
      completed: true,
      message: "Error: Nip must have only numbers.",
    };
  let orgData = {
    orgName: formData.get("name"),
    nip: nip === "" ? null : parseInt(nip),
    street: formData.get("street"),
    city: formData.get("city"),
    postalCode: formData.get("postal"),
    creditLimit: credit === "" ? null : parseInt(credit),
    countryId: formData.get("country"),
  };

  if (
    !validators.lengthSmallerThen(orgData.orgName, 50) ||
    !validators.stringIsNotEmpty(orgData.orgName)
  )
    return {
      error: true,
      completed: true,
      message: "Error: Org name is empty or length exceed 50 chars.",
    };

  if (
    !validators.lengthSmallerThen(orgData.street, 200) ||
    !validators.stringIsNotEmpty(orgData.street)
  )
    return {
      error: true,
      completed: true,
      message: "Error: Street is empty or length exceed 200 chars.",
    };

  if (
    !validators.lengthSmallerThen(orgData.city, 200) ||
    !validators.stringIsNotEmpty(orgData.city)
  )
    return {
      error: true,
      completed: true,
      message: "Error: City is empty or length exceed 200 chars.",
    };

  if (
    !validators.lengthSmallerThen(orgData.postalCode, 200) ||
    !validators.stringIsNotEmpty(orgData.postalCode)
  )
    return {
      error: true,
      completed: true,
      message: "Error: Postal code is empty or length exceed 25 chars.",
    };

  const dbName = await getDbName();
  const userId = await getUserId();
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

  if (info.ok) {
    let orgId = await info.text();

    let statusData = {
      orgId: parseInt(orgId),
      statusId: parseInt(formData.get("availability")),
    };

    const statusInfo = await fetch(
      `${process.env.API_DEST}/${dbName}/Client/setAvailabilityStatusesToClient?userId=${userId}`,
      {
        method: "POST",
        body: JSON.stringify(statusData),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );
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
  } else {
    return {
      error: true,
      completed: true,
      message: "Critical error",
    };
  }
}
