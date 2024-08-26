import PropTypes from "prop-types";
import Image from "next/image";
import { Stack, Container, Button } from "react-bootstrap";
import Link from "next/link";
import NotificationBadge from "../smaller_components/notification_icon";
import UserSettingWindow from "./user_setting_window";
import small_logo from "../../../public/small_logo.png";
import sidebar_button from "../../../public/icons/sidebar_icon.png";
import { usePathname, useSearchParams } from "next/navigation";

function MenuTemplate({
  children,
  sidebar_action,
  user_name,
  user_window_active_link = "none",
  current_nofitication_qty,
}) {
  const params = useSearchParams();
  const pathName = usePathname();
  const accessaibleParams = new URLSearchParams(params);
  const pagation = accessaibleParams.get("pagation")
    ? accessaibleParams.get("pagation")
    : "";
  const finalParam = pagation != "" ? `?pagation=${pagation}` : "";
  const menuSize = {
    height: "81px",
  };
  return (
    <Stack className="px-3" direction="horizontal" style={menuSize}>
      <Container className="w-auto ms-0 ms-xl-3 me-0 me-xl-4">
        <Link href={`${pathName}${finalParam}`}>
          <Image src={small_logo} alt="logo" />
        </Link>
      </Container>
      <Container className="mx-auto ms-xl-0 text-center text-xl-start">
        {children}
      </Container>
      <Container className="ms-auto w-auto me-0 me-xl-3">
        <Button
          variant="as-link"
          className="pe-0 position-relative py-0 d-xl-none"
          onClick={sidebar_action}
        >
          <Image src={sidebar_button} alt="logo" />
          <NotificationBadge qty={current_nofitication_qty} />
        </Button>
        <Container className="pe-0 position-relative py-0 d-none d-xl-block">
          <UserSettingWindow
            user_name={user_name}
            notification_qty={current_nofitication_qty}
            active_link={user_window_active_link}
          />
        </Container>
      </Container>
    </Stack>
  );
}

MenuTemplate.PropTypes = {
  children: PropTypes.object,
  sidebar_action: PropTypes.func.isRequired,
  user_name: PropTypes.string.isRequired,
  user_window_active_link: PropTypes.string,
};

export default MenuTemplate;
