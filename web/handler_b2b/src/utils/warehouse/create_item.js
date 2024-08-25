"use server";

import getDbName from "../auth/get_db_name";
import validators from "../validators/validator";

export default async function createItem(eans, state, formData) {
  let data = {};

  try {
    let formatedEans = [];
    for (let index = 0; index < eans.length; index++) {
      formatedEans.push(parseInt(eans[index]));
    }
    data = {
      itemName: formData.get("name"),
      itemDescription: formData.get("description"),
      partNumber: formData.get("partNumber"),
      eans: formatedEans,
    };
  } catch (error) {
    console.log(error);
    return { error: true, complete: true };
  }
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
