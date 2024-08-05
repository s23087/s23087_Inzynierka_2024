import Image from "next/image";
import User_chooser from "../components/registration/user_type_input";
import gray_questionmark from "../../../../public/icons/gray_questionmark.png";

export default function Registration_Form() {
  return (
    <div className="grid gap-y-9 text-center">
      <div>
        <p className="text-black font-bold">Choose preferred user type:</p>
      </div>
      <div className="grid gap-y-6">
        <User_chooser is_org={false} />
        <User_chooser is_org={true} />
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
