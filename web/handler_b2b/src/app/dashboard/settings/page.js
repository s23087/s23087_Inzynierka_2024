"use server";

import { Stack } from "react-bootstrap";
import SettingMenu from "@/components/settings_components/Link_component/settings_menu_component";
import getRole from "@/utils/auth/get_role";

export default async function SettingsPage() {
  const current_role = await getRole();
  return (
    <Stack className="px-3 mx-2 mx-xl-4 pt-4">
      <SettingMenu role={current_role} />
    </Stack>
  );
}
