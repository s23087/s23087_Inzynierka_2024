"use server";

/**
 * Sends request to create new first user.
 * @param  {FormData} formData Contain form data.
 * @param  {string} dbName Database name.
 * @param  {boolean} isOrg Is the account organization type (true) or solo (false).
 * @return {Promise<boolean>}      Result of operation. If true success, otherwise failure.
 */
export default async function createNewRegisteredUser(formData, dbName, isOrg) {
  let nip = formData.get("nip");
  nip = nip === "" ? null : parseInt(nip);
  let data = {
    email: formData.get("email"),
    username: formData.get("name"),
    surname: formData.get("surname"),
    orgName: formData.get("company"),
    nip: nip,
    street: formData.get("street"),
    city: formData.get("city"),
    postalCode: formData.get("postal"),
    country: formData.get("country"),
    password: formData.get("password"),
    isOrg: isOrg === "true",
  };
  try {
    let response = await fetch(
      `${process.env.API_DEST}/${dbName}/Registration/registerUser`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      },
    );

    return response.ok;
  } catch {
    console.error("createNewRegisteredUser fetch failed.");
    return false;
  }
}
