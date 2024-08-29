"use server";

import ModifyUserForm from "@/components/settings_components/Forms_components/User_data_form";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getUserEmail from "@/utils/settings/get_email";

export default async function ChangeUserPage() {
  const userBasic = await getBasicInfo();
  const email = await getUserEmail();
  return (
    <ModifyUserForm
      email={email}
      name={userBasic.username}
      surname={userBasic.surname}
    />
  );
}
