"use server";

import { cookies } from "next/headers";
import SessionManagment from "./session_managment";

export default async function getUserId() {
  const cookie = cookies().get("session")?.value;
  const session = await SessionManagment.decrypt(cookie);

  return session.userId;
}
