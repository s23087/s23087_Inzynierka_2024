"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";
import logout from "../auth/logout";
import { redirect } from "next/navigation";

export default async function AddUser(state, formData) {
  let data = {
    email: formData.get("email"),
    username: formData.get("name"),
    surname: formData.get("surname"),
    password: formData.get("password"),
    roleName: formData.get("role"),
  };

  if (
    !validators.lengthSmallerThen(data.email, 350) ||
    !validators.stringIsNotEmpty(data.email) ||
    !validators.isEmail(data.email)
  )
    return {
      error: true,
      completed: true,
      message: "Error: Incorrect email or is empty. Max. 350 chars.",
    };

  if (
    !validators.lengthSmallerThen(data.username, 250) ||
    !validators.stringIsNotEmpty(data.username)
  )
    return {
      error: true,
      completed: true,
      message: "Error: Name is empty or length exceed 250 chars.",
    };

  if (
    !validators.lengthSmallerThen(data.surname, 250) ||
    !validators.stringIsNotEmpty(data.surname)
  )
    return {
      error: true,
      completed: true,
      message: "Error: Surname is empty or length exceed 250 chars.",
    };

  if (!validators.stringIsNotEmpty(data.password))
    return {
      error: true,
      completed: true,
      message: "Error: Postal code is empty or length exceed 25 chars.",
    };

  try {
    const dbName = await getDbName();
    const userId = await getUserId();
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Settings/add/user/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );
  
    if (info.status === 404) {
      logout();
      redirect("/");
    }
  
    if (info.status === 400) {
      let message = await info.text();
      return {
        error: true,
        message: message ? message : "Couldn't create the new user",
        completed: true,
      };
    }
  
    if (info.ok) {
      return {
        error: false,
        message: "Success! You have created a user.",
        completed: true,
      };
    }
  } catch {
    console.error("AddUser fetch failed.")
    return {
      error: false,
      message: "Connection error.",
      completed: true,
    };
  }
}
