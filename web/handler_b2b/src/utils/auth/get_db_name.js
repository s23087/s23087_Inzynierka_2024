"use server";

import { cookies } from "next/headers";
import SessionManagement from "./session_management";

/**
 * Decrypt cookie to get current user database name.
 * @return {Promise<string>} Name of user database
 */
export default async function getDbName() {
  const cookie = cookies().get("session")?.value;
  const session = await SessionManagement.decrypt(cookie);

  return session.dbName;
}
