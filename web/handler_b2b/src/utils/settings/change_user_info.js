"use server";

import getUserId from "../auth/get_user_id";
import getDbName from "../auth/get_db_name";
import logout from "../auth/logout";
import { redirect } from "next/navigation";

/**
 * Sends request to modify user profile information. When data is unchanged the attribute in request will be null.
 * @param  {object} prevState Object that contain information about previous state of chosen item.
 * @param  {object} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<object>}      Return object containing property: error {bool}, completed {bool} and message {string}. If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function changeUserInfo(prevState, state, formData) {
  const userId = await getUserId();
  const dbName = await getDbName();

  let data = {
    userId: userId,
    email:
      formData.get("email") === prevState.email ? null : formData.get("email"),
    username:
      formData.get("name") === prevState.name ? null : formData.get("name"),
    surname:
      formData.get("surname") === prevState.surname
        ? null
        : formData.get("surname"),
  };
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Settings/modify/user`,
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
      return {
        error: true,
        message: "This email exist alredy.",
        completed: true,
      };
    }

    if (info.ok) {
      return {
        error: false,
        completed: true,
        message: "Success! You have changed your info.",
      };
    }

    return {
      error: true,
      message: "Critical error.",
      completed: true,
    };
  } catch {
    console.error("changeUserInfo fetch failed.");
    return {
      error: true,
      message: "Connection error.",
      completed: true,
    };
  }
}
