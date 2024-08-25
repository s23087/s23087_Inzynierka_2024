"use server";

import { redirect } from "next/navigation";
import SessionManagment from "./session_managment";

export default async function logout() {
  SessionManagment.deleteSession();
  redirect("/");
}
