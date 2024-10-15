"use server";

import { NextResponse } from "next/server";

export async function GET() {
  try {
    const countries = await fetch(
      `${process.env.API_DEST}/template/Registration/countries`,
    ).then((res) => res.json());

    return Response.json({ countries });
  } catch {
    return NextResponse.json(
      { error: "Could not download countries" },
      { status: 500 },
    );
  }
}
