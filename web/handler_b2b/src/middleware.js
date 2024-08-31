"use server";

import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import SessionManagment from "./utils/auth/session_managment";

const protectedRoutes = /^.*dashboard.*$/;

export default async function middleware(req) {
  const path = req.nextUrl.pathname;
  const isProtectedRoute = protectedRoutes.test(path);

  const cookie = cookies().get("session")?.value;
  const session = await SessionManagment.decrypt(cookie);

  if (isProtectedRoute && !session?.userId) {
    return NextResponse.redirect(new URL("/", req.url));
  }

  if (!isProtectedRoute && session?.userId) {
    return NextResponse.redirect(new URL("/dashboard/warehouse", req.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|.*\\.png$).*)"],
};
