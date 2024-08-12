import PropTypes from "prop-types";
import Image from "next/image";
import { Stack, Container, Button } from "react-bootstrap";
import NotificationBadge from "../smaller_components/notification_icon";
import UserSettingWindow from "./user_setting_window";
import small_logo from "../../../public/small_logo.png";
import sidebar_button from "../../../public/icons/sidebar_icon.png";

function MenuTemplate({
  children,
  sidebar_action,
  user_name,
  user_window_active_link = "none",
}) {
  const menuSize = {
    height: "81px",
  };
  return (
    <Stack className="px-3" direction="horizontal" style={menuSize}>
      <Container className="w-auto ms-0 ms-xl-5 me-0 me-xl-4">
        <Image src={small_logo} alt="logo" />
      </Container>
      <Container className="mx-auto ms-xl-0 text-center text-xl-start">
        {children}
      </Container>
      <Container className="ms-auto w-auto me-0 me-xl-5">
        <Button
          variant="as-link"
          className="pe-0 position-relative py-0 d-xl-none"
          onClick={sidebar_action}
        >
          <Image src={sidebar_button} alt="logo" />
          <NotificationBadge qty={1} />
        </Button>
        <Container className="pe-0 position-relative py-0 d-none d-xl-block">
          <UserSettingWindow
            user_name={user_name}
            notification_qty={1}
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
