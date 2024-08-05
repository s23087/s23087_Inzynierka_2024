import Image from "next/image";
import user_icon from "../../../../../public/icons/user_icon.png";
import org_user_icon from "../../../../../public/icons/org_user_icon.png";

export default function User_chooser({ is_org }) {
  if (is_org) {
    return (
      <span className="bg-main_blue-primary hover:bg-main_blue-primary_hover rounded-md min-w-[259px] min-h-[122px] content-center">
        <div className="grid grid-cols-2">
          <div className="flex justify-center">
            <Image src={org_user_icon} alt="Org user icon"/>
          </div>
          <div className="inline content-center">
            <p className="text-start ml-2.5">Org User</p>
          </div>
        </div>
      </span>
    );
  }

  return (
    <span className="bg-main_blue-primary hover:bg-main_blue-primary_hover rounded-md min-w-[259px] min-h-[122px] content-center">
      <div className="grid grid-cols-3">
        <div className="flex justify-center ml-10">
          <Image src={user_icon} alt="User icon"/>
        </div>
        <div className="col-span-2 inline content-center ml-9">
          <p className="text-start">Individual User</p>
        </div>
      </div>
    </span>
  );
}
