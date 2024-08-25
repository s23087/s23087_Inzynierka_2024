import "server-only";
import { SignJWT, jwtVerify } from "jose";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";

const secretKey = process.env.SESSION_SECRET;
const encodedKey = new TextEncoder().encode(secretKey);

async function encrypt(payload) {
  return new SignJWT(payload)
    .setProtectedHeader({ alg: "HS256" })
    .setIssuedAt()
    .setExpirationTime("1d")
    .sign(encodedKey);
}

async function decrypt(session) {
  try {
    const { payload } = await jwtVerify(session, encodedKey, {
      algorithms: ["HS256"],
    });
    return payload;
  } catch (error) {
    return {};
  }
}

async function createSession(userId, role, dbName) {
  const expiresAt = new Date(Date.now() + 1 * 24 * 60 * 60 * 1000);
  const session = await encrypt({ userId, role, dbName, expiresAt });

  cookies().set("session", session, {
    httpOnly: true,
    secure: true,
    expires: expiresAt,
    sameSite: "lax",
    path: "/",
  });
}

function deleteSession() {
  cookies().delete("session");
}

async function verifySession() {
  const cookie = cookies().get("session").value;
  const session = await decrypt(cookie);

  if (!session.userId) {
    redirect("/");
  }

  return true;
}

const SessionManagment = {
  createSession,
  deleteSession,
  verifySession,
  decrypt,
};

export default SessionManagment;
