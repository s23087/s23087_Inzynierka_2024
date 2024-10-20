"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import validators from "../validators/validator";
import { getInvoicePath } from "./get_document_path";

export default async function updateInvoice(
  file,
  isYourInvoice,
  invoiceId,
  prevState,
  state,
  formData,
) {
  const fs = require("node:fs");
  let invoiceNumber = formData.get("invoice");
  let transport = formData.get("transport");
  let client = parseInt(formData.get("org"));
  let paymentMethod = parseInt(formData.get("paymentMethod"));
  let paymentStatus = parseInt(formData.get("paymentStatus"));
  let status = formData.get("status") === "true";
  let path = null;
  let note = formData.get("note");
  let message = validateData(transport, invoiceNumber);

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }

  transport = parseFloat(transport);
  const dbName = await getDbName();
  let prevPath = await getInvoicePath(invoiceId);

  if (invoiceNumber !== prevState.invoiceNumber || file) {
    if (!prevPath) {
      return {
        error: true,
        completed: true,
        message: "Error: Could not download file path from server.",
      };
    }
    try {
      if (file) {
        let buffArray = await file.get("file").arrayBuffer();
        let buff = new Uint8Array(buffArray);
        fs.writeFileSync(prevPath, buff);
      }
      if (invoiceNumber !== prevState.invoiceNumber) {
        let newPath = prevPath.replace(
          prevState.invoiceNumber
            .replaceAll(/[\\./]/g, "")
            .replaceAll(" ", "_"),
          invoiceNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_"),
        );
        path = newPath;
      }
    } catch (error) {
      return {
        error: true,
        completed: true,
        message: "Error: " + error,
      };
    }
  }

  let data = getData();

  try {
    const userId = await getUserId();
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Invoices/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 404) {
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
      if (data.invoiceNumber) {
        return await changePath(prevPath, data, dbName, userId, isYourInvoice, invoiceId, fs)
      }
      return {
        error: false,
        completed: true,
        message: "Success! You have modified the invoice.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error",
      };
    }
  } catch {
    console.error("updateInvoice fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error.",
    };
  }

  function getData() {
    return {
      isYourInvoice: isYourInvoice,
      invoiceId: invoiceId,
      invoiceNumber: invoiceNumber !== prevState.invoiceNumber ? invoiceNumber : null,
      clientId: client !== -1 ? client : null,
      transportCost: transport !== prevState.transport ? transport : null,
      paymentMethod: paymentMethod !== -1 ? paymentMethod : null,
      paymentStatus: paymentStatus !== -1 ? paymentStatus : null,
      inSystem: status !== prevState.status ? status : null,
      path: path ?? null,
      note: note !== prevState.note ? note : null,
    };
  }
}

async function changePath(prevPath, data, dbName, userId, isYourInvoice, invoiceId, fs) {
  try {
    fs.renameSync(prevPath, data.path);
  } catch (error) {
    const pathChange = await fetch(
      `${process.env.API_DEST}/${dbName}/Invoices/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify({
          isYourInvoice: isYourInvoice,
          invoiceId: invoiceId,
          path: prevPath,
        }),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );
    if (pathChange.ok) {
      return {
        error: true,
        completed: true,
        message: "Success with errors! The file has not been renamed.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error. Invoice has been updated but file not.",
      };
    }
  }
}

function validateData(transport, invoiceNumber) {
  let message = "Error:";
  if (
    !validators.isPriceFormat(transport) &&
    !validators.stringIsNotEmpty(transport)
  )
    message += "\nPrice must be a decimal and not empty.";
  if (
    !validators.lengthSmallerThen(invoiceNumber, 40) &&
    !validators.stringIsNotEmpty(invoiceNumber)
  )
    message += "\nInvoice number cannot be empty and excceed 40 chars.";
  return message;
}
