import Image from "next/image";
import bigLogo from "../../public/big_logo.png";
import Login_Form from "./ui/Login_form/login_form";

export default function Home() {
  return (
    <main className="flex flex-col min-h-screen items-center justify-center">
      <div className="mb-6">
        <Image 
          src={bigLogo}
          alt="Logo"
        />
      </div>

      <div className="mb-16">
        <Login_Form />
      </div>
    </main>
  );
}
