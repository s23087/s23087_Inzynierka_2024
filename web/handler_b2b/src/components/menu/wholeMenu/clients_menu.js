"use client";

import { useState } from "react";
import PropTypes from "prop-types";
import { Stack } from "react-bootstrap";
import PagePositionBar from "@/components/menu/page_position_bar";
import MenuTemplate from "@/components/menu/menu_template";
import CustomSidebar from "@/components/menu/sidebars/sidebar";
import Navlinks from "@/components/menu/navlinks";

/**
 * Create warehouse page menu.
 * @component
 * @param {Object} props
 * @param {string} props.current_role Current user role
 * @param {Number} props.current_notification_qty Number of unread notifications
 * @param {boolean} props.is_org_switch_needed True if org view switch is needed
 * @param {boolean} props.org_view True if org view is enabled
 * @param {{username: string, surname: string, orgName: string}} props.user Object containing user information
 * @return {JSX.Element} nav element
 */
function ClientMenu({
  current_role,
  current_notification_qty,
  is_org_switch_needed,
  org_view,
  user,
}) {
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
              active_link="Clients"
              notification_qty={current_notification_qty}
              is_sidebar={false}
            />
          </Stack>
        </Stack>
      </MenuTemplate>
      <PagePositionBar
        site_name="Clients"
        with_switch={is_org_switch_needed}
        switch_bool={org_view}
      />
      <CustomSidebar
        user_name={user.username + " " + user.surname}
        org_name={user.orgName}
        offcanvasShow={sidebarShow}
        onHideAction={() => setSidebarShow(false)}
      >
        <Navlinks
          role={current_role}
          active_link="Clients"
          notification_qty={current_notification_qty}
          is_sidebar={true}
        />
      </CustomSidebar>
    </nav>
  );
}

ClientMenu.propTypes = {
  current_role: PropTypes.string.isRequired,
  current_notification_qty: PropTypes.number.isRequired,
  is_org_switch_needed: PropTypes.bool.isRequired,
  org_view: PropTypes.bool.isRequired,
  user: PropTypes.object.isRequired,
};

export default ClientMenu;
