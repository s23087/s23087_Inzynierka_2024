"use server";

export default async function signIn(formData) {
  let data = {
    email: formData.get("email"),
    password: formData.get("password"),
  };

  let response = await fetch(
    `http://localhost:5226/${formData.get("companyId")}/User/sign_in`,
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
