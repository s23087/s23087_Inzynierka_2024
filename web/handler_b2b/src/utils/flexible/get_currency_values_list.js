"use server";

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
      return await info.json();
    }
  
    return {
      error: true,
      message: "Could not connect to nbp services.",
      rates: [],
    };
  } catch {
    console.error("getCurrencyValuesList fetch failed.")
    return {
      error: true,
      message: "Connection error.",
      rates: [],
    };
  }
}
