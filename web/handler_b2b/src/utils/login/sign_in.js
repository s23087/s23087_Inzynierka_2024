"use server";

import validators from "@/utils/validators/validator";
import { redirect } from "next/navigation";
import SessionManagment from "../auth/session_managment";

export default async function signIn(state, formData) {
  let data = {
    email: formData.get("email"),
    password: formData.get("password"),
  };
  let dbName = formData.get("companyId");

  if (
    !validators.isEmail(data.email) ||
    !validators.stringIsNotEmpty(data.email)
  ) {
    return {
      error: true,
      message: "Invalid email.",
    };
  }

  let response = await fetch(`${process.env.API_DEST}/${dbName}/User/sign_in`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (response.ok) {
    const body = await response.json();
    await SessionManagment.createSession(body.id, body.role, dbName);
    redirect("dashboard/warehouse");
  }

  return {
    error: true,
    message: "Your email or password is wrong.",
  };
}
