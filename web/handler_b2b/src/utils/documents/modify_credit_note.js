"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import getCreditPath from "./get_credit_path";

export default async function updateCreditNote(
  file,
  isYourCredit,
  creditNoteId,
  prevState,
  state,
  formData,
) {
  const fs = require("node:fs");
  let creditNoteNumber = formData.get("creditNumber");
  let date = formData.get("date");
  let status = formData.get("status") === "true";
  let isPaid = formData.get("isPaid") === "on";
  let path = null;
  let note = formData.get("note");
  let message = "Error:";
  if (
    !validators.lengthSmallerThen(creditNoteNumber, 40) &&
    !validators.stringIsNotEmpty(creditNoteNumber)
  )
    message += "\nInvoice number cannot be empty and excceed 40 chars.";

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }

  const dbName = await getDbName();
  let prevPath = await getCreditPath(creditNoteId);

  if (creditNoteNumber !== prevState.creditNoteNumber || file) {
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
      if (creditNoteNumber !== prevState.invoiceNumber) {
        let newPath = prevPath.replace(
          prevState.creditNoteNumber.replaceAll("/", ""),
          creditNoteNumber.replaceAll("/", ""),
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

  let data = {
    isYourCredit: isYourCredit,
    id: creditNoteId,
    creditNumber:
      creditNoteNumber !== prevState.creditNoteNumber ? creditNoteNumber : null,
    date: date !== prevState.date ? date : null,
    inSystem: status !== prevState.inSystem ? status : null,
    isPaid: isPaid !== prevState.isPaid ? isPaid : null,
    path: path ? path : null,
    note: note !== prevState.note ? note : null,
  };

  const userId = await getUserId();
  const info = await fetch(
    `${process.env.API_DEST}/${dbName}/CreditNote/modify?userId=${userId}`,
    {
      method: "POST",
      body: JSON.stringify(data),
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  if (info.status === 404) {
    let infoText = await info.text();
    if (infoText.includes("User")) logout();
    return {
      error: true,
      completed: true,
      message: infoText,
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
    if (data.creditNumber) {
      try {
        fs.renameSync(prevPath, data.path);
      } catch (error) {
        const pathChange = await fetch(
          `${process.env.API_DEST}/${dbName}/CreditNote/modify?userId=${userId}`,
          {
            method: "POST",
            body: JSON.stringify({
              isYourCredit: isYourCredit,
              id: creditNoteId,
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
    return {
      error: false,
      completed: true,
      message: "Success! You have modified the credit note.",
    };
  } else {
    return {
      error: true,
      completed: true,
      message: "Critical error",
    };
  }
}