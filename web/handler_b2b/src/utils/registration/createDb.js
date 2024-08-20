"use server";

export default async function initDb(dbName) {
  const isCreated = await fetch(
    `http://localhost:5226/template/Registration/createDb?orgName=${dbName}`,
    { method: "POST" },
  );

  if (isCreated.ok) {
    const isSetup = await fetch(
      `http://localhost:5226/${dbName}/Registration/setupDb`,
      { method: "POST" },
    );

    return isSetup.ok;
  }

  return false;
}
