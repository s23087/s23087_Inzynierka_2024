"use server";

import ModifyUserOrgForm from "@/components/settings_components/Forms_components/Org_modify_form";
import getOrgModifyInfo from "@/utils/flexible/get_Org_modify_info";
import getCountries from "@/utils/flexible/get_countries";

export default async function ChangeOrgPage() {
  const orgInfo = await getOrgModifyInfo();
  const countries = await getCountries();
  return <ModifyUserOrgForm orgInfo={orgInfo} countries={countries} />;
}
