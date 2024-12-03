"use server";

/**
 * Sends request to create new database with given name.
 * @param  {string} dbName Database name.
 * @return {Promise<boolean>} Return true if success, otherwise false.
 */
export default async function initDb(dbName) {
  try {
    const isCreated = await fetch(
      `${process.env.API_DEST}/template/Registration/createDb/${dbName}`,
      { method: "POST" },
    );

    if (isCreated.ok) {
      const isSetup = await fetch(
        `${process.env.API_DEST}/${dbName}/Registration/setupDb`,
        { method: "POST" },
      );

      return isSetup.ok;
    }
    return false;
  } catch (error) {
    console.error(error);
    console.error("initDb fetch failed.");
    return false;
  }
}
