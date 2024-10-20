"use server";

import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import SessionManagment from "./utils/auth/session_managment";
import getRole from "./utils/auth/get_role";

const protectedRoutes = /^.*dashboard.*$/;
const restrictedPagesSolo =
  /^.*docType=Requests$|^.*\/roles.*$|^.*\/settings\/add_user.*$/;
const restrictedPagesMerchant =
  /^.*\/roles.*$|^.*\/settings\/add_user.*$|^.*\/dashboard\/abstract_items.*$|^.*\/settings\/change_organization.*$/;
const restrictedPagesAccountant =
  /^.*\/roles.*$|^.*\/pricelist.*$|^.*\/settings\/add_user.*$|^.*\/settings\/change_organization.*$/;
const allowedPagesWarehouseManagerPartOne =
  /^.*\/deliveries.*$|^.*\/notifications.*$|^.*\/settings.*$/;
const allowedPagesWarehouseManagerPartTwo =
  /^.*\/change_password.*$|^.*\/change_data.*$|^.*\/unauthorized.*$/;

function checkRestriction(role, url) {
  switch (role) {
    case "Solo":
      return restrictedPagesSolo.test(url);
    case "Merchant":
      return restrictedPagesMerchant.test(url);
    case "Accountant":
      return restrictedPagesAccountant.test(url);
    case "Warehouse Manager":
      return (
        !allowedPagesWarehouseManagerPartOne.test(url) ||
        !allowedPagesWarehouseManagerPartTwo.test(url)
      );
    default:
      return false;
  }
}

export default async function middleware(req) {
  const path = req.nextUrl.href;
  const isProtectedRoute = protectedRoutes.test(path);

  const cookie = cookies().get("session")?.value;
  const session = await SessionManagment.decrypt(cookie);

  if (isProtectedRoute && !session?.userId) {
    return NextResponse.redirect(new URL("/", req.url));
  }

  if (!isProtectedRoute && session?.userId) {
    return NextResponse.redirect(new URL("/dashboard/warehouse", req.url));
  }

  const role = await getRole();
  if (checkRestriction(role, path)) {
    return NextResponse.redirect(new URL("/dashboard/unauthorized", req.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|.*\\.png$).*)"],
};
