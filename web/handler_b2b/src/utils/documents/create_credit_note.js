"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to create credit note.
 * @param  {{userOrg: string, chosenClient: string, isYourCreditNote: boolean, invoiceId: Number}} additionalInvoiceInfo Object containing properties additional information about invoice.
 * @param  {Array<{id: Number, invoiceId: Number, priceId: Number, qty: Number, price: Number, purchasePrice: Number}>} products Products added to credit note.
 * @param  {FormData} file FormData object containing file binary data.
 * @param  {{error: boolean, completed: boolean, message: string}} state Previous state of object bonded to this function.
 * @param  {FormData} formData Contain form data.
 * @return {Promise<{error: boolean, completed: boolean, message: string}>} If error is true that action was unsuccessful.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
export default async function CreateCreditNote(
  additionalInvoiceInfo,
  products,
  file,
  state,
  formData,
) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let creditNumber = formData.get("creditNumber");
  let date = formData.get("date");
  let errorMessage = validateData(
    additionalInvoiceInfo.invoiceId,
    creditNumber,
    date,
    products,
    file,
  );

  if (errorMessage.length > 6)
    return {
      error: true,
      completed: true,
      message: errorMessage,
    };

  const dbName = await getDbName();
  const userId = await getUserId();
  let chosenUser = parseInt(formData.get("user"));

  let creditDate = formData.get("date").replaceAll("-", "_");
  let fileName = `../../database/${dbName}/documents/cn_${creditNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_")}_${userId}${additionalInvoiceInfo.userOrg.replaceAll(" ", "_")}${additionalInvoiceInfo.chosenClient.replaceAll(" ", "_")}_${creditDate}_${Date.now().toString()}.pdf`;
  let transformProducts = [];
  products.forEach((element) => {
    transformProducts.push({
      userId: chosenUser,
      itemId: element.id,
      invoiceId: element.invoiceId,
      purchasePriceId: element.priceId,
      qty: additionalInvoiceInfo.isYourCreditNote
        ? element.qty
        : Math.abs(element.qty),
      newPrice: element.price,
    });
    if (element.qty > 0) {
      transformProducts.push({
        userId: chosenUser,
        itemId: element.id,
        invoiceId: element.invoiceId,
        purchasePriceId: element.priceId,
        qty: element.qty * -1,
        newPrice: element.purchasePrice,
      });
    }
  });

  let creditNoteData = getData();

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

  /**
   * Organize information into object for fetch.
   * @return {object}
   */
  function getData() {
    return {
      creditNoteNumber: creditNumber,
      creditNoteDate: formData.get("date"),
      inSystem: formData.get("status") === "true",
      isPaid: formData.get("isPaid") === "on",
      note: formData.get("description") ? formData.get("description") : "",
      invoiceId: additionalInvoiceInfo.invoiceId,
      isYourCreditNote: additionalInvoiceInfo.isYourCreditNote,
      filePath: fileName,
      creditNoteItems: transformProducts,
    };
  }
}

/**
 * Validate form data.
 * @param  {string} invoiceId Invoice id.
 * @param  {string} creditNumber String containing credit note number.
 * @param  {string} date Date in string.
 * @param  {Array<object>} products Products added to proforma.
 * @param  {object} file File object.
 * @return {string}      Error message. If no error occurred only "Error:" is returned.
 */
function validateData(invoiceId, creditNumber, date, products, file) {
  let errorMessage = "Error:";
  if (!invoiceId) {
    errorMessage += "\nInvoice must not be empty.";
  }
  if (
    !validators.lengthSmallerThen(creditNumber, 40) ||
    !validators.stringIsNotEmpty(creditNumber)
  )
    errorMessage += "\nCredit note number must not be empty.";
  if (new Date(date) > Date.now())
    errorMessage +=
      "\nCannot create credit note with date exceeding todays's date.";
  if (products.length <= 0) errorMessage += "\nCredit note must have products.";
  if (!file) errorMessage += "\nDocument must not be empty.";
  return errorMessage;
}
