"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

export default async function createDeliveryCompany(state, formData) {
  let companyName = formData.get("name");
  if (!companyName)
    return {
      error: true,
      completed: true,
      message: "Company name must not be empty or excceed 40 chars.",
    };

  const userId = await getUserId();
  const dbName = await getDbName();

  let data = {
    companyName: companyName,
  };

  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/Delivery/company/add?userId=${userId}`,
    {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );
  if (info.status === 404) {
    let text = await info.text();
    if (text === "User not found.") {
      logout();
    }
    return {
      error: true,
      completed: true,
      message: "Your user profile does not exists.",
    };
  }
  if (info.status === 400) {
    return {
      error: true,
      completed: true,
      message: await info.text(),
    };
  }

  if (info.ok) {
    return {
      error: false,
      completed: true,
      message: "Success! You had added new company.",
    };
  } else {
    return {
      error: true,
      completed: true,
      message: "Critical error.",
    };
  }
}
