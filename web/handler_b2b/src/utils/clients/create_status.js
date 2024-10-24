"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
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
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Client/add/availability_status/${userId}`,
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
        message: "Your account does not exists.",
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
    console.error("Create status fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error",
    };
  }
}
