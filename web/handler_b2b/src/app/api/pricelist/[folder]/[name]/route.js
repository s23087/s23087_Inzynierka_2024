import { notFound } from "next/navigation";

export async function GET(request, { params }) {
  const folderName = params.folder;
  const fileName = params.name;
  const fs = require("node:fs");
  try {
    const xmlContent = fs.readFileSync(`src/app/api/pricelist/${folderName}/${fileName}`);
    return new Response(xmlContent, {
      headers: { "Content-Type": "text/xml" },
    });
  } catch {
    return notFound();
  }
}
