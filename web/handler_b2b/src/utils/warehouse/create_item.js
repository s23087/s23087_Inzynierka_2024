"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";

export default async function createItem(eans, state, formData) {
  const userId = await getUserId();
  let data = {
    userId: userId,
    itemName: formData.get("name"),
    itemDescription: formData.get("description"),
    partNumber: formData.get("partNumber"),
    eans: eans,
  };

  if (
    !validators.lengthSmallerThen(data.itemName, 250) ||
    !validators.stringIsNotEmpty(data.itemName)
  )
    return { error: true, complete: true };
  if (
    !validators.lengthSmallerThen(data.itemDescription, 500) ||
    !validators.stringIsNotEmpty(data.itemDescription)
  )
    return { error: true, complete: true };
  if (
    !validators.lengthSmallerThen(data.partNumber, 150) ||
    !validators.stringIsNotEmpty(data.partNumber)
  )
    return { error: true, complete: true };

  const dbName = await getDbName();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Warehouse/addItem`,
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
    };
  }
}
