"use server";

import { redirect } from "next/navigation";
import SessionManagement from "./session_management";

/**
 * Logout current user and redirect to login site.
 */
export default async function logout() {
  SessionManagement.deleteSession();
  redirect("/");
}
