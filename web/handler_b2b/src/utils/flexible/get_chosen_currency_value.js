"use server";

export default async function getCurrencyValueByDate(currency, date) {
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
  } catch {
    console.error("getCurrencyValueByDate fetch failed");
    return null;
  }
}
