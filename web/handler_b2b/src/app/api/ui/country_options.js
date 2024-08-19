"use client";

import useSWR from "swr";

const fetcher = (...args) => fetch(...args).then((res) => res.json());

export default function CountryOptions() {
  const { data, error, isLoading } = useSWR("/api/countries", fetcher);

  if (error) return <option>Critical Error</option>;
  if (isLoading) return <option>Loading...r</option>;

  let values = Object.values(data.countries);

  return values.map((e) => {
    return <option key={e.countryName}>{e.countryName}</option>;
  });
}
