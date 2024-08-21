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

  let response = await fetch(
    `http://localhost:5226/${dbName}/Registration/registerUser`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    },
  );

  if (response.ok) {
    return true;
  }

  return false;
}
