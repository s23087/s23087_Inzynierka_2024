"use server";

import { redirect } from "next/navigation";
import initDb from "./createDb";
import createNewRegisteredUser from "./registerUser";

function createFolders(dbfile, logFile, docFile, pricelistFile) {
  const fs = require("node:fs");
  try {
    if (!fs.existsSync(dbfile)) {
      fs.mkdirSync(dbfile);
      fs.mkdirSync(logFile);
      fs.mkdirSync(docFile);
      fs.mkdirSync(pricelistFile);
      return true;
    }
    return false;
  } catch (error) {
    console.log("Error:" + error);
    return false;
  }
}

function deleteFolders(dbfile, logFile, docFile, pricelistFile) {
  const fs = require("node:fs");
  try {
    fs.rmdirSync(logFile);
    fs.rmdirSync(dbfile);
    fs.rmdirSync(docFile);
    fs.rmdirSync(pricelistFile);
  } catch (error) {
    console.log("Error:" + error);
  }
}

async function registerUser(is_org, formData) {
  let orgName = formData.get("company");
  let folderName = orgName.replace(/[^a-zA-Z0-9]/, "");
  folderName = folderName.replace(" ", "");
  let dbFilePath = "../../database/".concat(folderName);
  let dbLogPath = dbFilePath.concat("/log");
  let docPath = dbFilePath.concat("/documents");
  let pricelistFile = `src/app/api/pricelist/${folderName}`;
  let fileCreation = false;

  fileCreation = createFolders(dbFilePath, dbLogPath, docPath, pricelistFile);

  if (fileCreation) {
    let creationResult = await initDb(folderName);
    if (!creationResult) {
      redirect("failure");
    }
    let registerResult = await createNewRegisteredUser(
      formData,
      folderName,
      is_org,
    );
    if (registerResult) {
      redirect("success");
    }
    deleteFolders(dbFilePath, dbLogPath, docPath, pricelistFile);
    redirect("failure");
  } else {
    redirect("failure");
  }
}

export default registerUser;
