"use server";

export async function GET() {
  const countries = await fetch(
    `${process.env.API_DEST}/template/Registration/countries`,
  ).then((res) => res.json());

  return Response.json({ countries });
}
