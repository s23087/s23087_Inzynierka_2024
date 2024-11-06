"use server";

/**
 * Read file from the server then return it's data in json format.
 * @return {Promise<Object>}      File data in json format. If connection was lost return null.
 */
export default async function getFileFormServer(path) {
  const fs = require("node:fs");
  try {
    let file = fs.readFileSync(path);
    return JSON.stringify(file);
  } catch (ex) {
    console.error(ex);
    return null;
  }
}
