"use server";

import validators from "@/utils/validators/validator";
import { redirect } from "next/navigation";
import SessionManagement from "../auth/session_management";

/**
 * Sends request to authorize user. If authorization is success it will redirect user to dashboard. If not it will return error object.
 * @param  {{error: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, message: string}>} Return action result with message. If error is true that action was unsuccessful.
 */
export default async function signIn(state, formData) {
  let data = {
    email: formData.get("email"),
    password: formData.get("password"),
  };
  let dbName = formData.get("companyId").replaceAll(/[\\./=+]/g, "");
  if (!dbName) return { error: true, message: "Company id is invalid." };
  const fs = require("node:fs");
  try {
    if (!fs.existsSync(`src/app/api/pricelist/${dbName}`)) {
      return {
        error: true,
        message: "Invalid company id.",
      };
    }
  } catch (error) {
    console.log(error);
    return {
      error: true,
      message: "Server error.",
    };
  }

  if (
    !validators.isEmail(data.email) ||
    !validators.stringIsNotEmpty(data.email)
  ) {
    return {
      error: true,
      message: "Invalid email.",
    };
  }
  let response;
  try {
    response = await fetch(`${process.env.API_DEST}/${dbName}/User/sign_in`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      return {
        error: true,
        message: "Your email or password is wrong.",
      };
    }

    if (response.status == 500) {
      return {
        error: true,
        message: "The company id is incorrect.",
      };
    }
  } catch (error) {
    console.error(error);
    console.error("signIn fetch failed");
    return {
      error: true,
      message: "Connection error.",
    };
  }
  const body = await response.json();
  await SessionManagement.createSession(body.id, body.role, dbName);
  if (body.role === "Warehouse Manager") redirect("dashboard/deliveries");
  redirect("dashboard/warehouse");
}
