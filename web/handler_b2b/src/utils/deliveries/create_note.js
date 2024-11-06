"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";

/**
 * Sends request to create delivery note.
 * @param  {Number} deliveryId Delivery id.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function createNote(deliveryId, state, formData) {
  const dbName = await getDbName();
  const userId = await getUserId();
  let note = formData.get("note");
  if (
    !validators.stringIsNotEmpty(note) ||
    !validators.lengthSmallerThen(note, 500)
  )
    return {
      error: true,
      completed: true,
      message: "Error: Note must not be empty or exceed 500 chars.",
    };
  let data = {
    note: note,
    userId: userId,
    deliveryId: deliveryId,
  };

  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Delivery/add/note`,
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
      return {
        error: true,
        completed: true,
        message: "Your account does not exist.",
      };
    }
    if (info.status === 400) {
      logout();
      return {
        error: true,
        completed: true,
        message: await info.text(),
      };
    }
    if (info.status === 500) {
      return {
        error: true,
        completed: true,
        message: "Server error",
      };
    }

    if (info.ok) {
      return {
        error: false,
        completed: true,
      };
    }
    return {
      error: true,
      completed: true,
      message: "Critical error",
    };
  } catch {
    console.error("createNote fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
