"use server";

export default async function initDb(dbName) {
  const isCreated = await fetch(
    `${process.env.API_DEST}/template/Registration/createDb?orgName=${dbName}`,
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
}
