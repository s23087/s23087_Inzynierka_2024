export async function GET() {
  const countries = await fetch(
    "http://localhost:5226/template/Registration/countries",
  ).then((res) => res.json());

  return Response.json({ countries });
}
