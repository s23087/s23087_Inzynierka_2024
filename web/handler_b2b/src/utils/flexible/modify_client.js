"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";

export default async function modifyClient(orgId, prevState, state, formData) {
  const userId = await getUserId();
  let nip = formData.get("nip");
  let credit = formData.get("credit");
  let messageError = "Errors:"

  if (!orgId)
    messageError += "\n Org Id not found"
  if (
    !validators.lengthSmallerThen(formData.get("name"), 50) ||
    !validators.stringIsNotEmpty(formData.get("name"))
  )
    messageError += "\n Org name is empty or length exceed 50 chars."

  if (
    !validators.lengthSmallerThen(formData.get("street"), 200) ||
    !validators.stringIsNotEmpty(formData.get("street"))
  )
    messageError += "\n Street is empty or length exceed 200 chars."

  if (
    !validators.lengthSmallerThen(formData.get("city"), 200) ||
    !validators.stringIsNotEmpty(formData.get("city"))
  )
    messageError += "\n City is empty or length exceed 200 chars."

  if (
    !validators.lengthSmallerThen(formData.get("postal"), 200) ||
    !validators.stringIsNotEmpty(formData.get("postal"))
  )
    messageError += "\n Postal code is empty or length exceed 25 chars."

  if (messageError.length > 7){
    return {
      error: true,
      completed: true,
      message: messageError,
    };
  }

  let data = {
    orgId: orgId,
    orgName:
      formData.get("name") === prevState.orgName ? null : formData.get("name"),
    nip: nip === "" ? null : parseInt(nip),
    street:
      formData.get("street") === prevState.street
        ? null
        : formData.get("street"),
    city: formData.get("city") === prevState.city ? null : formData.get("city"),
    postalCode:
      formData.get("postal") === prevState.postalCode
        ? null
        : formData.get("postal"),
    creditLimit: credit ? parseInt(credit) : null,
    countryId: formData.get("country"),
  };

  const dbName = await getDbName();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Client/modifyOrg?userId=${userId}`,
    {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  if (info.status === 404) {
    return {
      error: true,
      completed: true,
      message: "Error: User or Org id not found.",
    };
  }
  if (info.ok) {
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

  return {
    error: true,
    completed: true,
    message: "Critical error: Contact the tech support.",
  };
}
