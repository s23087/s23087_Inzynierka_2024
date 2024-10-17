"use client";

import PropTypes from "prop-types";
import { Stack } from "react-bootstrap";
import MenuTemplate from "@/components/menu/menu_template";
import PagePostionBar from "@/components/menu/page_position_bar";
import CustomSidebar from "@/components/menu/sidebars/sidebar";
import Navlinks from "@/components/menu/navlinks";
import { useState } from "react";

function SettingNavMenu({ current_role, current_nofitication_qty, user }) {
  const [sidebarShow, setSidebarShow] = useState(false);
  const showSidebar = () => setSidebarShow(true);
  const hideSidebar = () => setSidebarShow(false);
  return (
    <nav className="fixed-top main-bg z-1 border-bottom-grey">
      <MenuTemplate
        sidebar_action={showSidebar}
        user_name={user.username + " " + user.surname}
        user_window_active_link="Settings"
        current_nofitication_qty={current_nofitication_qty}
      >
        <Stack className="ps-xl-2" direction="horizontal" gap={4}>
          <Stack className="d-none d-xl-flex" direction="horizontal" gap={4}>
            <Navlinks
              role={current_role}
              active_link="Settings"
              notification_qty={current_nofitication_qty}
              is_sidebar={false}
            />
          </Stack>
        </Stack>
      </MenuTemplate>
      <PagePostionBar
        site_name="Settings"
        with_switch={false}
        switch_bool={false}
      />
      <CustomSidebar
        user_name={user.username + " " + user.surname}
        org_name={user.orgName}
        offcanvasShow={sidebarShow}
        onHideAction={hideSidebar}
      >
        <Navlinks
          role={current_role}
          active_link="Settings"
          notification_qty={current_nofitication_qty}
          is_sidebar={true}
        />
      </CustomSidebar>
    </nav>
  );
}

SettingNavMenu.propTypes = {
  current_role: PropTypes.string.isRequired,
  current_nofitication_qty: PropTypes.number.isRequired,
  user: PropTypes.object.isRequired,
};

export default SettingNavMenu;
