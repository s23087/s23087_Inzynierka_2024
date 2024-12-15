"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";

/**
 * Sends request to modify chosen item. When data is unchanged the attribute in request will be null.
 * @param  {Array} eans Any array of string containing ean values.
 * @param  {{itemId: number, name: string, description: string, partNumber: string}} prevState Object that contain information about previous state of chosen item.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function updateItem(eans, prevState, state, formData) {
  let message = validateData(formData);

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }

  const userId = await getUserId();
  let data = {
    userId: userId,
    id: prevState.itemId,
    itemName:
      formData.get("name") === prevState.name ? null : formData.get("name"),
    itemDescription:
      formData.get("description") === prevState.description
        ? null
        : formData.get("description"),
    partNumber:
      formData.get("partNumber") === prevState.partNumber
        ? null
        : formData.get("partNumber"),
    eans: eans,
  };
  try {
    const dbName = await getDbName();
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Warehouse/modify`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 500) {
      return {
        error: true,
        completed: true,
        message: "Server error.",
      };
    }

    if (info.status === 400) {
      return {
        error: true,
        completed: true,
        message: "This part number exist.",
      };
    }

    if (info.status === 404) {
      return {
        error: true,
        completed: true,
        message: "This item doesn't exist.",
      };
    }

    if (info.ok) {
      return {
        error: false,
        completed: true,
        message: "Success! Item has been modified.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error",
      };
    }
  } catch {
    console.error("updateItem fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error",
    };
  }
}
/**
 * Validate form data.
 * @param  {FormData} formData Contain form data.
 * @return {string} Return error message. "Error:" means no error occurred.
 */
function validateData(formData) {
  let message = "Error:";
  if (
    !validators.lengthSmallerThen(formData.get("name"), 250) ||
    !validators.stringIsNotEmpty(formData.get("name"))
  )
    message += "\nName is empty or exceed required length";

  if (
    !validators.lengthSmallerThen(formData.get("description"), 500)
  )
    message += "\nDescription or exceed required length";

  if (
    !validators.lengthSmallerThen(formData.get("partNumber"), 150) ||
    !validators.stringIsNotEmpty(formData.get("partNumber"))
  )
    message += "\nPartnumber is empty or exceed required length";
  return message;
}
