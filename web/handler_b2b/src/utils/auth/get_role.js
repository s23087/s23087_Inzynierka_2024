"use server";

import { cookies } from "next/headers";
import SessionManagement from "./session_management";

/**
 * Decrypt session cookie to get current user role. If not found return empty string.
 * @return {Promise<string>} User role.
 */
export default async function getRole() {
  const cookie = cookies().get("session")?.value;
  const session = await SessionManagement.decrypt(cookie);

  return session.role ? session.role : "";
}
