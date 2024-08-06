import Image from "next/image";
import UserChooser from "../components/registration/user_type_input";
import gray_questionmark from "../../../../public/icons/gray_questionmark.png";

export default function RegistrationForm() {
  return (
    <div className="grid gap-y-9 text-center">
      <div>
        <p className="text-black font-bold">Choose preferred user type:</p>
      </div>
      <div className="grid gap-y-6">
        <UserChooser is_org={false} />
        <UserChooser is_org={true} />
      </div>
      <div className="flex justify-center">
        <Image src={gray_questionmark} alt="Question icon"/>
        <p className="text-gray-secondary text-xs ml-2">
          Click here to learn about differences
        </p>
      </div>
    </div>
  );
}
