"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";

/**
 * Sends request to change client - user bindings.
 * @param  {number} orgId Organization id.
 * @param  {Array<Number>} userIds Array of user ids.
 * @return {Promise<{ok: boolean, message: string}>} Return result of action in object. If ok is true, then operation is success, otherwise false.
 */
export default async function setUserClientBindings(orgId, userIds) {
  let data = {
    orgId: orgId,
    usersId: userIds,
  };

  const dbName = await getDbName();
  const userId = await getUserId();
  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Client/modify/user_client_bindings/${userId}`,
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
      if (text === "Your account does not exists.") logout();
      return {
        ok: false,
        message: text,
      };
    }

    if (info.ok) {
      return {
        ok: true,
        message: "Success!",
      };
    } else {
      let text = await info.text();
      return {
        ok: false,
        message: text,
      };
    }
  } catch {
    console.error("Set user client bindings fetch failed.");
    return {
      ok: false,
      message: "Connection error.",
    };
  }
}
