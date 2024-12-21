import "server-only";
import { SignJWT, jwtVerify } from "jose";
import { cookies } from "next/headers";

/**
 * Loaded secret key from env.
 */
const secretKey = process.env.SESSION_SECRET;
/**
 * Encrypted secret key
 */
const encodedKey = new TextEncoder().encode(secretKey);

/**
 * Create signed Json web token using algorithm HS256 that expire after 1 day.
 * @param {{ userId: Number, role: string, dbName: string, expiresAt: Date }} payload Data to encrypt.
 * @return {Promise<SignJWT>} Signed JWT token
 */
async function encrypt(payload) {
  return new SignJWT(payload)
    .setProtectedHeader({ alg: "HS256" })
    .setIssuedAt()
    .setExpirationTime("1d")
    .sign(encodedKey);
}

/**
 * Decrypt given JWT token and return it's data. If error occurred return object without properties.
 * @param {SignJWT} session Current session token.
 * @return {Promise<{ userId: Number, role: string, dbName: string, expiresAt: Date }>} Decrypted token payload.
 */
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

/**
 * Create cookie session with given data. Data is encrypted in JWT and given to cookie. It's expire after 1 day.
 * @param {Number} userId Id of user that signed in.
 * @param {string} role Role of user that signed in.
 * @param {string} dbName Database name that user organization is using.
 */
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

/**
 * Delete session cookie. That will logout the user.
 */
function deleteSession() {
  cookies().delete("session");
}

async function verifySession() {
  const cookie = cookies().get("session").value;
  const session = await decrypt(cookie);

  if (!session.userId) {
    return false;
  }

  return true;
}

const SessionManagement = {
  createSession,
  deleteSession,
  verifySession,
  decrypt,
};

export default SessionManagement;
