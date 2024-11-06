import "./globals.css";
import { Roboto } from "next/font/google";

// Main font
const roboto = Roboto({
  weight: "400",
  subsets: ["latin"],
});

export const metadata = {
  title: "HandlerB2B",
  description: "",
};
/**
 * Main html body
 */
export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body className={roboto.className}>{children}</body>
    </html>
  );
}
