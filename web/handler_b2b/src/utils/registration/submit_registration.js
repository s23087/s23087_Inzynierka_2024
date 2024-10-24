"use server";

import { redirect } from "next/navigation";
import initDb from "./createDb";
import createNewRegisteredUser from "./registerUser";

/**
 * Create database, log, document and pricelist folder with given names.
 * @param  {string} dbfile Database file name.
 * @param  {string} logFile Database log file name.
 * @param  {string} docFile Document file name.
 * @param  {string} pricelistFile Pricelist file name.
 * @return {boolean}      Return true if no error occurred. Otherwise false
 */
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
  } catch {
    console.error("createFolders fetch failed.");
    return false;
  }
}

/**
 * Delete database, log, document and pricelist folder with given names.
 * @param  {string} dbfile Database file name.
 * @param  {string} logFile Database log file name.
 * @param  {string} docFile Document file name.
 * @param  {string} pricelistFile Pricelist file name.
 * @return {boolean}      Return true if no error occurred. Otherwise false
 */
function deleteFolders(dbfile, logFile, docFile, pricelistFile) {
  const fs = require("node:fs");
  try {
    fs.rmdirSync(logFile);
    fs.rmdirSync(dbfile);
    fs.rmdirSync(docFile);
    fs.rmdirSync(pricelistFile);
  } catch {
    console.error("deleteFolders fetch failed.");
  }
}

/**
 * Sends request to create new database and first user. Depend on result will redirect user to appropriate site.
 * @param  {boolean} is_org Any array of string containing ean values.
 * @param  {FormData} formData Contain form data.
 */
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
    try {
      let creationResult = await initDb(folderName);
      if (!creationResult) {
        throw new Error("Db creation failed")
      }
      let registerResult = await createNewRegisteredUser(
        formData,
        folderName,
        is_org,
      );
      if (!registerResult) {
        throw new Error("Registration failed")
      }
    } catch (error) {
      deleteFolders(dbFilePath, dbLogPath, docPath, pricelistFile);
      redirect("failure");
    }
    redirect("success");
  } else {
    console.log("file failure");
    redirect("failure");
  }
}

export default registerUser;
