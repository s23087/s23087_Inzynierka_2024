"use server";

/**
 * Sends request to get chosen currency values from NPB site within specified date range. If start day is not week day then it will take as starting date the last friday from this date.
 * @param  {string} currency Shortcut name of currency.
 * @param  {string} startDate in format "yyyy-MM-dd"
 * @param  {string} endDate in format "yyyy-MM-dd"
 * @return {Promise<{error: boolean, message: string, rates: Array<{no: string, effectiveDate: string, mid: Number}>}>}      Object containing information about currency and result if operation was a success with message. If connection is lost returns null.
 */
export default async function getCurrencyValuesList(
  currency,
  startDate,
  endDate,
) {
  let url = "";
  let curr = currency.toLowerCase();
  let newDate = new Date(startDate);
  if (newDate.getDay() === 0 || newDate.getDay() === 6) {
    if (newDate.getDay() === 0) {
      newDate.setDate(newDate.getDate() - 2);
    } else {
      newDate.setDate(newDate.getDate() - 1);
    }
    url = `https://api.nbp.pl/api/exchangerates/rates/a/${curr}/${newDate.toLocaleDateString("en-CA")}/${endDate}/?format=json`;
  } else {
    url = `https://api.nbp.pl/api/exchangerates/rates/a/${curr}/${startDate}/${endDate}/?format=json`;
  }
  try {
    const info = await fetch(url, {
      method: "GET",
    });

    if (info.ok) {
      let rates = await info.json();
      return {
        error: false,
        message: "Success",
        rates: rates.rates,
      };
    }

    return {
      error: true,
      message: "Could not connect to nbp services.",
      rates: [],
    };
  } catch (error) {
    console.error(error);
    console.error("getCurrencyValuesList fetch failed.");
    return {
      error: true,
      message: "Connection error.",
      rates: [],
    };
  }
}
