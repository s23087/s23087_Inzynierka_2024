"use server";

/**
 * Sends request to get chosen currency value from NPB site.
 * @param  {string} currency Shortcut name of currency.
 * @return {Promise<table: string, currency: string, code: string, rates: Array<{no: string, effectiveDate: string, mid: Number}>>}      Json object containing information about currency. If connection is lost returns null.
 */
export default async function getCurrencyValues(currency) {
  let curr = currency.toLowerCase();
  let url = `https://api.nbp.pl/api/exchangerates/rates/a/${curr}/last?format=json`;
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      return await info.json();
    }

    return {};
  } catch {
    console.error("getCurrencyValues fetch failed.");
    return null;
  }
}
