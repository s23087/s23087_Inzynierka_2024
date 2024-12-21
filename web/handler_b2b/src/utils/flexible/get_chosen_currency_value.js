"use server";

import SessionManagement from "../auth/session_management";
import { NextResponse } from "next/server";

/**
 * Sends request to get chosen currency value from NPB site within specified date.
 * @param  {string} currency Shortcut name of currency.
 * @param  {string} date in format "yyyy-MM-dd"
 * @return {Promise<Number>}      Currency value. If connection is lost returns null. If not found returns 0.
 */
export default async function getCurrencyValueByDate(currency, date) {
  let userAuthorized = await SessionManagement.verifySession();
  if (!userAuthorized) NextResponse.redirect(new URL("/"));
  let curr = currency.toLowerCase();
  let url = `https://api.nbp.pl/api/exchangerates/rates/a/${curr}/${date}/?format=json`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      let result = await info.json();
      return result.rates[0].mid;
    }

    return 0;
  } catch (error) {
    console.error(error);
    console.error("getCurrencyValueByDate fetch failed");
    return null;
  }
}
