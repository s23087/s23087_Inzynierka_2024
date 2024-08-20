"use server";

import { redirect } from "next/navigation";
import initDb from "./createDb";

function createFolders(dbfile, logFile) {
  const fs = require("node:fs");
  try {
    if (!fs.existsSync(dbfile)) {
      fs.mkdirSync(dbfile);
      fs.mkdirSync(logFile);
      return true;
    }
    return false;
  } catch (error) {
    console.log(error);
    return false;
  }
}

function deleteFolders(dbfile, logFile) {
  const fs = require("node:fs");
  try {
    fs.rmdirSync(logFile);
    fs.rmdirSync(dbfile);
  } catch (error) {
    console.log(error);
  }
}

async function registerUser(formData) {
  let orgName = formData.get("company");
  let folderName = orgName.replace(/[^a-zA-Z0-9]/, "");
  folderName = folderName.replace(" ", "");
  let dbFilePath = "../../database/".concat(folderName);
  let dbLogPath = dbFilePath.concat("/log");
  let fileCreation = false;

  fileCreation = createFolders(dbFilePath, dbLogPath);

  if (fileCreation) {
    let creationResult = await initDb(folderName);
    if (creationResult) {
      redirect("success");
    }
    deleteFolders(dbFilePath, dbLogPath);
    redirect("failure");
  } else {
    redirect("failure");
  }
}

export default registerUser;
