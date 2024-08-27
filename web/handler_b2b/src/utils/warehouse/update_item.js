"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";

export default async function updateItem(eans, prevState, state, formData) {
  if (
    !validators.lengthSmallerThen(formData.get("name"), 250) ||
    !validators.stringIsNotEmpty(formData.get("name"))
  )
    return {
      error: true,
      complete: true,
      message: "Name is empty or exceed required lenght",
    };
  if (
    !validators.lengthSmallerThen(formData.get("description"), 500) ||
    !validators.stringIsNotEmpty(formData.get("description"))
  )
    return {
      error: true,
      complete: true,
      message: "Description is empty or exceed required lenght",
    };
  if (
    !validators.lengthSmallerThen(formData.get("partNumber"), 150) ||
    !validators.stringIsNotEmpty(formData.get("partNumber"))
  )
    return {
      error: true,
      complete: true,
      message: "Partnumber is empty or exceed required lenght",
    };

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

  const dbName = await getDbName();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Warehouse/modifyItem`,
    {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  if (info.ok) {
    return {
      error: false,
      complete: true,
    };
  } else {
    return {
      error: true,
      complete: true,
      message: "Status error: " + info.status,
    };
  }
}
