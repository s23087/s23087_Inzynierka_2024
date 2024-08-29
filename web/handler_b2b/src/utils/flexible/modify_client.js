"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";

export default async function modifyClient(orgId, state, formData) {
  const userId = await getUserId();
  let nip = formData.get("nip");
  let data = {
    orgId: orgId,
    orgName: formData.get("name"),
    nip: nip === "" ? null : parseInt(nip),
    street: formData.get("street"),
    city: formData.get("city"),
    postalCode: formData.get("postal"),
    countryId: formData.get("country"),
  };

  if (!data.orgId)
    return {
      error: true,
      complete: true,
      message: "Critical error: Org Id not found",
    };
  if (
    !validators.lengthSmallerThen(data.orgName, 50) ||
    !validators.stringIsNotEmpty(data.orgName)
  )
    return {
      error: true,
      complete: true,
      message: "Error: Org name is empty or length exceed 50 chars.",
    };

  if (
    !validators.lengthSmallerThen(data.street, 200) ||
    !validators.stringIsNotEmpty(data.street)
  )
    return {
      error: true,
      complete: true,
      message: "Error: Street is empty or length exceed 200 chars.",
    };

  if (
    !validators.lengthSmallerThen(data.city, 200) ||
    !validators.stringIsNotEmpty(data.city)
  )
    return {
      error: true,
      complete: true,
      message: "Error: City is empty or length exceed 200 chars.",
    };

  if (
    !validators.lengthSmallerThen(data.postalCode, 200) ||
    !validators.stringIsNotEmpty(data.postalCode)
  )
    return {
      error: true,
      complete: true,
      message: "Error: Postal code is empty or length exceed 25 chars.",
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

  console.log(data);
  console.log(info.status);

  if (info.status === 404) {
    return {
      error: true,
      complete: true,
      message: "Error: User or Org id not found.",
    };
  }
  if (info.ok) {
    return {
      error: false,
      complete: true,
    };
  }

  return {
    error: true,
    complete: true,
    message: "Critical error: Contact the tech support.",
  };
}
