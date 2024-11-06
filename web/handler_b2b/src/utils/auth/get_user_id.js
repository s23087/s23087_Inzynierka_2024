"use server";

import { cookies } from "next/headers";
import SessionManagement from "./session_management";

/**
 * Decrypt session cookie to get current user id. If not found return null.
 * @return {Promise<Number>} User id.
 */
export default async function getUserId() {
  const cookie = cookies().get("session")?.value;
  const session = await SessionManagement.decrypt(cookie);

  return session.userId ?? null;
}
