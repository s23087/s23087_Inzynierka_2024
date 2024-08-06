import Image from "next/image";
import bigLogo from "../../../public/big_logo.png";
import RegistrationForm from "../ui/Registration_form/registration_form";

export default function Registration() {
  return (
    <main className="flex flex-col min-h-screen items-center justify-center">
      <div className="mb-6">
        <Image src={bigLogo} alt="Logo"/>
      </div>

      <div className="mb-16">
        <RegistrationForm />
      </div>
    </main>
  );
}
