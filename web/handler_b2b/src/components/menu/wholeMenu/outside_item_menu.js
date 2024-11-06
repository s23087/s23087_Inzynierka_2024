"use client";

import { useState } from "react";
import PropTypes from "prop-types";
import { Stack } from "react-bootstrap";
import MenuTemplate from "@/components/menu/menu_template";
import CustomSidebar from "@/components/menu/sidebars/sidebar";
import Navlinks from "@/components/menu/navlinks";
import AbstractItemPositionBar from "@/components/smaller_components/abstract_items_bar";

/**
 * Create outside items page menu.
 * @component
 * @param {Object} props
 * @param {string} props.current_role Current user role
 * @param {Number} props.current_notification_qty Number of unread notifications
 * @param {{username: string, surname: string, orgName: string}} props.user Object containing user information
 * @return {JSX.Element} nav element
 */
function OutsideItemsMenu({ current_role, current_notification_qty, user }) {
  // useState for showing sidebar offcanvas
  const [sidebarShow, setSidebarShow] = useState(false);
  return (
    <nav className="fixed-top main-bg z-1">
      <MenuTemplate
        sidebar_action={() => setSidebarShow(true)}
        user_name={user.username + " " + user.surname}
        current_notification_qty={current_notification_qty}
      >
        <Stack className="ps-xl-2" direction="horizontal" gap={4}>
          <Stack className="d-none d-xl-flex" direction="horizontal" gap={4}>
            <Navlinks
              role={current_role}
              active_link=""
              notification_qty={current_notification_qty}
              is_sidebar={false}
            />
          </Stack>
        </Stack>
      </MenuTemplate>
      <AbstractItemPositionBar site_name="Abstract items" />
      <CustomSidebar
        user_name={user.username + " " + user.surname}
        org_name={user.orgName}
        offcanvasShow={sidebarShow}
        onHideAction={() => setSidebarShow(false)}
      >
        <Navlinks
          role={current_role}
          active_link=""
          notification_qty={current_notification_qty}
          is_sidebar={true}
        />
      </CustomSidebar>
    </nav>
  );
}

OutsideItemsMenu.propTypes = {
  current_role: PropTypes.string.isRequired,
  current_notification_qty: PropTypes.number.isRequired,
  user: PropTypes.object.isRequired,
};

export default OutsideItemsMenu;
