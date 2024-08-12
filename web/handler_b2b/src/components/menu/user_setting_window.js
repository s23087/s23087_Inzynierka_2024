"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { Container, Button, Stack } from "react-bootstrap";
import Image from "next/image";
import Link from "next/link";
import NotificationBadge from "../smaller_components/notification_icon";
import user_settings_icon from "../../../public/icons/user_settings_icon.png";

function UserSettingWindow({
  user_name,
  notification_qty,
  active_link = "none",
}) {
  const [showWindow, setShowWindow] = useState(false);
  const openWindow = () => setShowWindow(true);
  const closeWindow = () => setShowWindow(false);
  const windowStyle = {
    width: "260px",
    height: "220px",
    "border-radius": "10px",
    display: showWindow ? "block" : "none",
  };
  const containerStyle = {
    width: "52px",
    height: "51px",
  };
  const buttonStyle = {
    width: "214px",
  };
  return (
    <Container className="p-0 m-0 position-relative" style={containerStyle}>
      <Button
        variant="as-link"
        className="p-0 position-absolute z-2"
        onClick={showWindow ? closeWindow : openWindow}
      >
        <Image src={user_settings_icon} alt="logo" />
        <NotificationBadge qty={notification_qty} />
      </Button>
      <Container
        className="position-absolute top-50 end-50 main-bg border-full-blue px-4 z-1"
        style={windowStyle}
      >
        <Stack className="pt-3" gap={3}>
          <p className="mb-2">{user_name}</p>
          {active_link.toLowerCase() === "settings" ? (
            <p className="mb-0 blue-sec-text text-decoration-none fst-italic">
              Settings
            </p>
          ) : (
            <Link
              href="/dashboard/settings"
              className="blue-main-text text-decoration-none"
            >
              Settings
            </Link>
          )}
          <Container className="d-flex px-0">
            {active_link.toLowerCase() === "notifications" ? (
              <p className="mb-0 blue-sec-text text-decoration-none fst-italic">
                Notifications
              </p>
            ) : (
              <Link
                href="/dashboard/notifications"
                className="blue-main-text text-decoration-none"
              >
                Notifications
              </Link>
            )}
            <Container className="position-relative">
              <NotificationBadge
                qty={notification_qty}
                top_value="0.5px"
                right_value="0"
              />
            </Container>
          </Container>
          <Button variant="mainBlue" style={buttonStyle} className="mt-2">
            Log out
          </Button>
        </Stack>
      </Container>
    </Container>
  );
}

UserSettingWindow.PropTypes = {
  user_name: PropTypes.string.isRequired,
  notification_qty: PropTypes.number.isRequired,
  active_link: PropTypes.string,
};

export default UserSettingWindow;
