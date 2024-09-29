"use server";

export default async function getFileFormServer(path) {
  const fs = require("node:fs");
  try {
    let file = fs.readFileSync(path);
    return JSON.stringify(file);
  } catch (ex) {
    console.log(ex);
    return JSON.stringify(file);
  }
}
