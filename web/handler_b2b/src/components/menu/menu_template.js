import PropTypes from "prop-types";
import Image from "next/image";
import { Stack, Container, Button } from "react-bootstrap";
import Link from "next/link";
import NotificationBadge from "../smaller_components/notification_icon";
import UserSettingWindow from "./user_setting_window";
import small_logo from "../../../public/small_logo.png";
import sidebar_button from "../../../public/icons/sidebar_icon.png";
import { usePathname, useSearchParams } from "next/navigation";

/**
 * Create Temple for menu.
 * @component
 * @param {Object} props
 * @param {Object} props.children Elements tha will be placed between logo and user icon
 * @param {Function} props.sidebar_action Function that will be triggered after clicking open side bar menu button
 * @param {string} props.user_name Full username (name + surname)
 * @param {string} [props.user_window_active_link="none"] Name of current chosen link (ex. if user is in roles page then roles will be active link)
 * @param {Number} props.current_notification_qty Number of unread notification
 * @return {JSX.Element} Stack element
 */
function MenuTemplate({
  children,
  sidebar_action,
  user_name,
  user_window_active_link = "none",
  current_notification_qty,
}) {
  const params = useSearchParams();
  const pathName = usePathname();
  const accessibleParams = new URLSearchParams(params);
  // Current pagination
  const pagination = accessibleParams.get("pagination")
    ? accessibleParams.get("pagination")
    : "";
  // Pagination param for icon return to home link
  const finalParam = pagination != "" ? `?pagination=${pagination}` : "";
  // Styles
  const menuSize = {
    height: "81px",
  };
  return (
    <Stack className="px-3" direction="horizontal" style={menuSize}>
      <Container className="w-auto ms-0 ms-xl-3 me-0 me-xl-4">
        <Link href={`${pathName}${finalParam}`}>
          <Image src={small_logo} alt="logo" priority={true} quality={100}/>
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
          <Image src={sidebar_button} alt="menu" priority={true} quality={100}/>
          <NotificationBadge qty={current_notification_qty} />
        </Button>
        <Container className="pe-0 position-relative py-0 d-none d-xl-block">
          <UserSettingWindow
            user_name={user_name}
            notification_qty={current_notification_qty}
            active_link={user_window_active_link}
          />
        </Container>
      </Container>
    </Stack>
  );
}

MenuTemplate.propTypes = {
  children: PropTypes.object,
  sidebar_action: PropTypes.func.isRequired,
  user_name: PropTypes.string.isRequired,
  user_window_active_link: PropTypes.string,
  current_notification_qty: PropTypes.number.isRequired,
};

export default MenuTemplate;
