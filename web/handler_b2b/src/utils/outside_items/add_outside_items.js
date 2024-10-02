"use server";

import { Readable } from "node:stream";
import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";

export default async function addOutsideItems(file, state, formData) {
  const userId = await getUserId();
  const dbName = await getDbName();
  let errorMessage = "Error:";
  let chosenOrg = formData.get("org");
  let chosenCurrency = formData.get("currency");
  if (!chosenCurrency) errorMessage += "\nCurrency must not be empty.";
  if (!chosenOrg) errorMessage += "\nOrganization must not be empty.";
  if (!file) errorMessage += "\nYou must provide a csv file.";

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  let data = {
    orgId: parseInt(chosenOrg),
    currency: chosenCurrency,
    items: [],
  };

  let dataValidator = (row) => {
    let error = 0;
    if (!row.partnumber) error++;
    if (!row.itemName) error++;
    if (row.ean) {
      let eans = row.ean.split(",");
      eans.forEach((element) => {
        if (!validators.haveOnlyNumbers(element)) {
          error++;
        }
      });
    }
    if (!validators.haveOnlyNumbers(row.qty)) error++;
    if (!validators.isPriceFormat(row.price)) error++;
    return error === 0;
  };

  const fs = require("node:fs");
  const { parse } = require("csv-parse");
  let buffArray = await file.get("file").arrayBuffer();
  let buff = new Uint8Array(buffArray);
  const readable = new Readable();
  readable._read = () => {};
  readable.push(buff);
  readable.push(null);
  let errorRows = [];
  let index = 2;
  readable
    .pipe(
      parse({
        delimiter: ",",
        columns: ["partnumber", "itemName", "itemDesc", "ean", "qty", "price"],
        from_line: 2,
      }),
    )
    .on("data", function (row) {
      if (dataValidator(row)) {
        data.items.push(row);
      } else {
        errorRows.push(index);
      }
      index++;
    })
    .on("error", async () => {
      await fetch(
        `${process.env.API_DEST}/${dbName}/OutsideItem/add/error/notification?userId=${userId}`,
        {
          method: "POST",
          body: JSON.stringify({
            info: "Import failed. The file must have exactly 6 columns.",
          }),
          headers: {
            "Content-Type": "application/json",
          },
        },
      );
    })
    .on("end", async () => {
      const info = await fetch(
        `${process.env.API_DEST}/${dbName}/OutsideItem/add?userId=${userId}`,
        {
          method: "POST",
          body: JSON.stringify(data),
          headers: {
            "Content-Type": "application/json",
          },
        },
      );
      if (info.status == 404) {
        logout();
      }
      if (errorRows.length > 0) {
        let message = `Import info. ${errorRows.length} rows has been omitted. Indexes: ${errorRows.join(",")}`;
        if (message.length > 250) message = message.substring(0, 247) + "...";
        await fetch(
          `${process.env.API_DEST}/${dbName}/OutsideItem/add/error/notification?userId=${userId}`,
          {
            method: "POST",
            body: JSON.stringify({
              info: message,
            }),
            headers: {
              "Content-Type": "application/json",
            },
          },
        );
      }
    });
  return {
    error: false,
    completed: true,
    message: "The file has been uploaded, wait for futher notifications.",
  };
}
