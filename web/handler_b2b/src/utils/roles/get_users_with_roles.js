"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import { redirect } from "next/navigation";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get users roles. Can be filtered or sorted using sort and role arguments. If encounter error will redirect you to correct site. If encounter error 404 will logout user.
 * @param  {string} search Searched phrase.
 * @param  {string} sort Name of attribute that users will be sorted. First char indicates direction. D for descending and A for ascending. This parameter cannot be omitted.
 * @param  {string} role String that contains role name that user will be filtered by. This parameter cannot be omitted.
 * @return {Promise<Array<{userId: Number, username: string, surname: string, roleName: string}>>}      Array of objects that contain user information. If connection was lost return null. If error occurred return empty array.
 */
export default async function getUserRoles(search, sort, role) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const dbName = await getDbName();
  const userId = await getUserId();
  let url;
  let params = [];
  if (sort !== ".None") params.push(`sort=${sort}`);
  if (role) params.push(`role=${role}`);
  if (search) {
    url = `${process.env.API_DEST}/${dbName}/Roles/get/${userId}&search=${search}${params.length > 0 ? "&" : ""}${params.join("&")}`;
  } else {
    url = `${process.env.API_DEST}/${dbName}/Roles/get/${userId}${params.length > 0 ? "?" : ""}${params.join("&")}`;
  }

  try {
    const data = await fetch(url, {
      method: "GET",
    });

    if (data.status === 404) {
      logout();
      redirect("/");
    }

    if (data.status === 400) {
      redirect("/dashboard/warehouse");
    }

    if (data.ok) {
      return await data.json();
    }

    return [];
  } catch {
    console.error("getUserRoles fetch failed.");
    return null;
  }
}
