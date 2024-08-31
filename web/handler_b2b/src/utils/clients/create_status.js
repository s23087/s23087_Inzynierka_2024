"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";

export default async function createStatus(state, formData) {
  let days = formData.get("days");
  if (!validators.haveOnlyNumbers(days) || !days)
    return {
      error: true,
      completed: true,
      message: "Error: Day must be a number and not empty",
    };
  let data = {
    statusName: formData.get("name"),
    daysForRealization: parseInt(days),
  };

  if (
    !validators.lengthSmallerThen(data.statusName, 150) ||
    !validators.stringIsNotEmpty(data.statusName)
  )
    return {
      error: true,
      completed: true,
      message: "Error: Status name is empty or length exceed 150 chars.",
    };

  const dbName = await getDbName();
  const userId = await getUserId();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Client/addAvailabilityStatuses?userId=${userId}`,
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
      completed: true,
    };
  }
  return {
    error: true,
    completed: true,
    message: "Critical error",
  };
}
