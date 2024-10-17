"use server";

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
  } catch {
    console.error("initDb fetch failed.")
    return false;
  }
}
