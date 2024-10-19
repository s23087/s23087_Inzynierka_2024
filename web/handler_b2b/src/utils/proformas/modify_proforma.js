"use server";

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id";
import logout from "../auth/logout";
import validators from "../validators/validator";
import getProformaPath from "./get_proforma_path";

export default async function updateProforma(
  file,
  orgs,
  prevState,
  proformaId,
  isYourProforma,
  state,
  formData,
) {
  const fs = require("node:fs");
  let user = parseInt(formData.get("user"));
  let proformaNumber = formData.get("proformaNumber");
  let note = formData.get("note");
  let org = formData.get("org");
  let transport = formData.get("transport");
  let paymentMethod = formData.get("paymentMethod");
  let status = formData.get("status") === "true";
  let path = "";
  let message = validateData(proformaNumber, user, org, transport);

  if (message.length > 6) {
    return {
      error: true,
      completed: true,
      message: message,
    };
  }
  const dbName = await getDbName();
  const userId = await getUserId();
  let prevPath = await getProformaPath(proformaId);

  if (file) {
    if (prevPath === null) {
      return {
        error: true,
        completed: true,
        message: "Error: Could not download file path from server.",
      };
    }
    try {
      if (file) {
        if (prevPath === "")
          prevPath = `../../database/${dbName}/documents/pr_${proformaNumber.replaceAll("/", "")}_${user}${orgs.userOrgId}${org}_${userId}${Date.now().toString()}.pdf`;
        let buffArray = await file.get("file").arrayBuffer();
        let buff = new Uint8Array(buffArray);
        fs.writeFileSync(prevPath, buff);
      }
      if (proformaNumber !== prevState.proformaNumber) {
        let newPath = prevPath.replace(
          prevState.proformaNumber
            .replaceAll(/[\\./]/g, "")
            .replaceAll(" ", "_"),
          proformaNumber.replaceAll(/[\\./]/g, "").replaceAll(" ", "_"),
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
    isYourProforma: isYourProforma,
    proformaId: proformaId,
    userId: parseInt(user) !== prevState.userId ? parseInt(user) : null,
    proformaNumber:
      proformaNumber !== prevState.proformaNumber ? proformaNumber : null,
    clientId: parseInt(org) !== -1 ? parseInt(org) : null,
    transport:
      parseFloat(transport) !== prevState.transport
        ? parseFloat(transport)
        : null,
    paymentMethodId:
      parseInt(paymentMethod) !== -1 ? parseInt(paymentMethod) : null,
    inSystem: status !== prevState.status ? status : null,
    path: path ? path : null,
    note: note !== prevState.note ? note : null,
  };

  try {
    const info = await fetch(
      `${process.env.API_DEST}/${dbName}/Proformas/modify/${userId}`,
      {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    if (info.status === 404) {
      let text = await info.text();
      if (text === "User not found.") logout();
      return {
        error: true,
        completed: true,
        message: text,
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
      if (data.proformaNumber && prevPath !== path) {
        try {
          fs.renameSync(prevPath, data.path);
        } catch (error) {
          const pathChange = await fetch(
            `${process.env.API_DEST}/${dbName}/Proformas/{userId}/modify`,
            {
              method: "POST",
              body: JSON.stringify({
                isYourProforma: isYourProforma,
                proformaId: proformaId,
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
        message: "Success! You have modified the request.",
      };
    } else {
      return {
        error: true,
        completed: true,
        message: "Critical error",
      };
    }
  } catch {
    console.error("updateProforma fetch failed.");
    return {
      error: true,
      completed: true,
      message: "Connection error",
    };
  }
}

function validateData(proformaNumber, user, org, transport) {
  let message = "Error:";
  if (!proformaNumber || proformaNumber.length > 40)
    message += "\nProfroma must not be empty or excceed 40 chars.";
  if (!user) message += "\nRecevier must not be empty.";
  if (!org) message += "\nClient must not be empty.";
  if (!transport || !validators.isPriceFormat(transport))
    message += "\nTransport must be a number and not empty.";
  return message;
}
