import PropTypes from "prop-types";
import Image from "next/image";
import { Stack, Container, Button } from "react-bootstrap";
import NotificationBadge from "../smaller_components/notification_icon";
import small_logo from "../../../public/small_logo.png";
import sidebar_button from "../../../public/icons/sidebar_icon.png";
import user_settings_icon from "../../../public/icons/user_settings_icon.png";

function MenuTemplate({ children, sidebar_action, user_window_action }) {
  const menuSize = {
    height: "81px",
  };
  return (
    <Stack className="px-3" direction="horizontal" style={menuSize}>
      <Container className="w-auto ms-0 ms-md-5">
        <Image src={small_logo} alt="logo" />
      </Container>
      <Container className="mx-auto text-center text-lg-start">
        {children}
      </Container>
      <Container className="ms-auto w-auto me-0 me-md-5">
        <Button
          variant="as-link"
          className="pe-0 position-relative py-0 d-lg-none"
          onClick={sidebar_action}
        >
          <Image src={sidebar_button} alt="logo" />
          <NotificationBadge qty={1} />
        </Button>
        <Button
          variant="as-link"
          className="pe-0 position-relative py-0 d-none d-lg-block"
          onClick={user_window_action}
        >
          <Image src={user_settings_icon} alt="logo" />
          <NotificationBadge qty={1} />
        </Button>
      </Container>
    </Stack>
  );
}

MenuTemplate.PropTypes = {
  children: PropTypes.object,
  sidebar_action: PropTypes.func.isRequired,
  user_window_action: PropTypes.func.isRequired,
};

export default MenuTemplate;
