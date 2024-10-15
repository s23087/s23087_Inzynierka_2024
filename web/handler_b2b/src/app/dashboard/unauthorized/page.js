"use server";

export default async function Unauthorized() {
  return (
    <main
      className="d-flex h-100 w-100 justify-content-center align-items-center"
      style={{ background: "black" }}
    >
      <h1 className="main-text">401 Unauthorized</h1>
    </main>
  );
}
