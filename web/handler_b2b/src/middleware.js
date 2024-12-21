"use server";

import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import SessionManagement from "./utils/auth/session_management";
import getRole from "./utils/auth/get_role";

/**
 * Regex that tells which sites unauthorized user cannot access.
 */
const protectedRoutes = /^.*dashboard.*$/;
/**
 * Regex that tells which sites "Solo" role cannot access.
 */
const restrictedPagesSolo =
  /^.*docType=Requests$|^.*\/roles.*$|^.*\/settings\/add_user.*$/;
/**
 * Regex that tells which sites "Merchant" role cannot access.
 */
const restrictedPagesMerchant =
  /^.*\/roles.*$|^.*\/settings\/add_user.*$|^.*\/dashboard\/abstract_items.*$|^.*\/settings\/change_organization.*$/;
/**
 * Regex that tells which sites "Accountant" role cannot access.
 */
const restrictedPagesAccountant =
  /^.*\/roles.*$|^.*\/pricelist.*$|^.*\/settings\/add_user.*$|^.*\/settings\/change_organization.*$/;
/**
 * Regex that tells which sites "Warehouse Manager" role cannot access. Part one.
 */
const allowedPagesWarehouseManagerPartOne =
  /^.*\/deliveries.*$|^.*\/notifications.*$|^.*\/settings.*$/;
/**
 * Regex that tells which sites "Warehouse Manager" role cannot access. Part two.
 */
const allowedPagesWarehouseManagerPartTwo =
  /^.*\/change_password.*$|^.*\/change_data.*$|^.*\/unauthorized.*$/;

/**
 * Checks if role can access url. Return always false if role is not matched to one of those: Solo, Merchant, Accountant, Warehouse Manager. This function is case sensitive.
 * @param  {string} role Role name.
 * @param  {string} url Accessed url.
 * @return {boolean}      True if role can access, otherwise false.
 */
function checkRestriction(role, url) {
  switch (role) {
    case "Solo":
      return restrictedPagesSolo.test(url);
    case "Merchant":
      return restrictedPagesMerchant.test(url);
    case "Accountant":
      return restrictedPagesAccountant.test(url);
    case "Warehouse Manager":
      if (allowedPagesWarehouseManagerPartOne.test(url)) return false;
      if (allowedPagesWarehouseManagerPartTwo.test(url)) return false;
      return true;
    default:
      return false;
  }
}

/**
 * Middleware that check if user can access chosen resources.
 * @param req Incoming Request
 */
export default async function middleware(req) {
  const path = req.nextUrl.href;
  const isProtectedRoute = protectedRoutes.test(path);

  const cookie = cookies().get("session")?.value;
  const session = await SessionManagement.decrypt(cookie);
  const isServerAction = req.headers.get("next-action");

  if (isServerAction) return NextResponse.next();

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
