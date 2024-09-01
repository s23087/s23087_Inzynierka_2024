"use server";

export default async function getCurrencyValues(currency) {
  let curr = currency.toLowerCase();
  let url = `https://api.nbp.pl/api/exchangerates/rates/a/${curr}/last?format=json`;
  const info = await fetch(url, {
    method: "GET",
  });

  if (info.ok) {
    return await info.json();
  }

  return {};
}
