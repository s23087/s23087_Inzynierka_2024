"use server";

import validators from "@/utils/validators/validator";
import { redirect } from "next/navigation";
import SessionManagment from "../auth/session_managment";

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
  } catch {
    console.error("signIn fetch failed")
    return {
      error: true,
      message: "Connection error.",
    };
  }
  const body = await response.json();
  await SessionManagment.createSession(body.id, body.role, dbName);
  if (body.role === "Warehouse Manager") redirect("dashboard/deliveries");
  redirect("dashboard/warehouse");
}
