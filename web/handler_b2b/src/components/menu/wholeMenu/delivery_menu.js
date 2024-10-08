"use client";

import { useState } from "react";
import PropTypes from "prop-types";
import { Stack, Container } from "react-bootstrap";
import PagePostionBar from "@/components/menu/page_position_bar";
import MenuTemplate from "@/components/menu/menu_template";
import CustomSidebar from "@/components/menu/sidebars/sidebar";
import Navlinks from "@/components/menu/navlinks";
import DeliverySwitch from "@/components/switches/deliveries_switch";

function DeliveryMenu({
  current_role,
  current_nofitication_qty,
  is_org_switch_needed,
  type,
  org_view,
  user,
}) {
  const [sidebarShow, setSidebarShow] = useState(false);
  const showSidebar = () => setSidebarShow(true);
  const hideSidebar = () => setSidebarShow(false);
  return (
    <nav className="fixed-top main-bg z-1">
      <MenuTemplate
        sidebar_action={showSidebar}
        user_name={user.username + " " + user.surname}
        current_nofitication_qty={current_nofitication_qty}
      >
        <Stack className="ps-xl-2" direction="horizontal" gap={4}>
          <Container className="mx-auto ms-xl-2 ms-xxl-0 me-xl-5 w-auto px-xl-0 px-xxl-2">
            <DeliverySwitch isDeliveryToUser={type === "Deliveries to user"} />
          </Container>
          <Stack className="d-none d-xl-flex" direction="horizontal" gap={4}>
            <Navlinks
              role={current_role}
              active_link="Deliveries"
              notification_qty={current_nofitication_qty}
              is_sidebar={false}
            />
          </Stack>
        </Stack>
      </MenuTemplate>
      <PagePostionBar
        site_name={
          type === "Deliveries to clients"
            ? "Deliveries to clients"
            : "Deliveries to user"
        }
        with_switch={is_org_switch_needed}
        switch_bool={org_view}
      />
      <CustomSidebar
        user_name={user.username + " " + user.surname}
        org_name={user.orgName}
        offcanvasShow={sidebarShow}
        onHideAction={hideSidebar}
      >
        <Navlinks
          role={current_role}
          active_link="Deliveries"
          notification_qty={current_nofitication_qty}
          is_sidebar={true}
        />
      </CustomSidebar>
    </nav>
  );
}

DeliveryMenu.PropTypes = {
  current_role: PropTypes.string.isRequired,
  current_nofitication_qty: PropTypes.number.isRequired,
  is_org_switch_needed: PropTypes.bool.isRequired,
  type: PropTypes.string.isRequired,
  org_view: PropTypes.bool.isRequired,
  user: PropTypes.object.isRequired,
};

export default DeliveryMenu;