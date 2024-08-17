import { Stack } from "react-bootstrap";
import SettingMenu from "@/components/settings_components/Link_component/settings_menu_component";

export default function SettingsPage() {
  const current_role = "Admin";
  return (
    <Stack className="px-4 px-xl-5 mx-xl-4 pt-4">
      <SettingMenu role={current_role} />
    </Stack>
  );
}
