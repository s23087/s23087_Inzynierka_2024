import { notFound } from "next/navigation";

/**
 * Get name of folder and file from parameters and return file xml content.
 * @param {Object} request Object representing request
 * @param {Object} param
 * @param {Object} param.params Params passed using dynamic routes
 * @param {string} param.params.folder Folder name which will be organizations ids
 * @param {string} param.params.name File name which will be extracted from pricelist paths
 */
export async function GET(request, { params }) {
  const folderName = params.folder;
  const fileName = params.name;
  const fs = require("node:fs");
  try {
    const xmlContent = fs.readFileSync(
      `src/app/api/pricelist/${folderName}/${fileName}`,
    );
    return new Response(xmlContent, {
      headers: { "Content-Type": "text/xml" },
    });
  } catch {
    return notFound();
  }
}
