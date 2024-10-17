"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";

export default async function CreateCreditNote(
  userOrg,
  choosenClient,
  isYourCreditNote,
  invoiceId,
  products,
  file,
  state,
  formData,
) {
  let errorMessage = "Error:";
  if (!invoiceId) {
    errorMessage += "\nInvoice must not be empty.";
  }

  let creditNumber = formData.get("creditNumber");
  if (
    !validators.lengthSmallerThen(creditNumber, 40) ||
    !validators.stringIsNotEmpty(creditNumber)
  )
    errorMessage += "\nCredit note number must not be empty.";
  let date = formData.get("date");
  if (new Date(date) > Date.now())
    errorMessage += "\nTransport cost must not be empty and must be decimal.";
  if (products.length <= 0) errorMessage += "\nCredit note must have products.";
  if (!file) errorMessage += "\nDocument must not be empty.";

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  const dbName = await getDbName();
  const userId = await getUserId();
  let choosenUser = parseInt(formData.get("user"));

  let creditDate = formData.get("date").replaceAll("-", "_");
  let fileName = `../../database/${dbName}/documents/cn_${creditNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_")}_${userId}${userOrg}${choosenClient}_${creditDate}_${Date.now().toString()}.pdf`;
  let transformProducts = [];
  products.forEach((element) => {
    transformProducts.push({
      userId: choosenUser,
      itemId: element.id,
      invoiceId: element.invoiceId,
      purchasePriceId: element.priceId,
      qty: isYourCreditNote ? element.qty : Math.abs(element.qty),
      newPrice: element.price,
    });
    if (element.qty > 0) {
      transformProducts.push({
        userId: choosenUser,
        itemId: element.id,
        invoiceId: element.invoiceId,
        purchasePriceId: element.priceId,
        qty: element.qty * -1,
        newPrice: element.purchasePrice,
      });
    }
  });

  let creditNoteData = {
    creditNotenumber: creditNumber,
    creditNoteDate: formData.get("date"),
    inSystem: formData.get("status") === "true",
    isPaid: formData.get("isPaid") === "on",
    note: formData.get("description") ? formData.get("description") : "",
    invoiceId: invoiceId,
    isYourCreditNote: isYourCreditNote,
    filePath: fileName,
    creditNoteItems: transformProducts,
  };

  const fs = require("node:fs");
  try {
    if (fs.existsSync(fileName)) {
      return {
        error: true,
        completed: true,
        message: "That document already exist.",
      };
    }
    let buffArray = await file.get("file").arrayBuffer();
    let buff = new Uint8Array(buffArray);
    fs.writeFileSync(fileName, buff);
  } catch (error) {
    console.log(error);
    return {
      error: true,
      completed: true,
      message: "Could not upload file.",
    };
  }

  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/CreditNote/add/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(creditNoteData),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 404) {
      logout();
      return {
        error: true,
        completed: true,
        message: "Unauthorized.",
      };
    }

    if (info.status === 400) {
      return {
        error: true,
        completed: true,
        message: await info.text(),
      };
    }

    if (info.status === 500) {
      return {
        error: true,
        completed: true,
        message: "Server error.",
      };
    }

    if (info.ok) {
      return {
        error: false,
        completed: true,
        message: "Success! You had created credit note.",
      };
    } else {
      try {
        fs.rmSync(fileName);
      } catch (error) {
        console.log(error);
        return {
          error: true,
          completed: true,
          message: "Critical file error.",
        };
      }
      return {
        error: true,
        completed: true,
        message: "Critical error.",
      };
    }
  } catch {
    console.error("Create credit note fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }
}
