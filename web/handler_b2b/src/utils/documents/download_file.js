"use server";

import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Read file from the server then return it's data in json format.
 * @return {Promise<Object>}      File data in json format. If connection was lost return null.
 */
export default async function getFileFormServer(path) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  const fs = require("node:fs");
  try {
    let file = fs.readFileSync(path);
    return JSON.stringify(file);
  } catch (ex) {
    console.error(ex);
    return null;
  }
}
