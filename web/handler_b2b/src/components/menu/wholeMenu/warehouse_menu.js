"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { Stack, Container } from "react-bootstrap";
import MenuTemplate from "../menu_template";
import CurrencyChangeButton from "@/components/smaller_components/currency_change_button";
import Navlinks from "../navlinks";
import PagePostionBar from "../page_position_bar";
import CustomSidebar from "../sidebars/sidebar";
import CurrencyOffcanvas from "@/components/offcanvas/currency_offcanvas";

function WarehouseMenu({
  current_role,
  current_nofitication_qty,
  is_org_switch_needed,
  org_view,
  user,
  currency,
}) {
  const [sidebarShow, setSidebarShow] = useState(false);
  const showSidebar = () => setSidebarShow(true);
  const hideSidebar = () => setSidebarShow(false);
  const [currencyShow, setCurrencyShow] = useState(false);
  return (
    <nav className="fixed-top main-bg z-1">
      <MenuTemplate
        sidebar_action={showSidebar}
        user_name={user.username + " " + user.surname}
        current_nofitication_qty={current_nofitication_qty}
      >
        <Stack className="ps-xl-2" direction="horizontal" gap={4}>
          <Container className="mx-auto ms-xl-2 ms-xxl-0 me-xl-5 w-auto px-xl-0 px-xxl-2">
            <CurrencyChangeButton
              currency={currency}
              openCurrencyOffcanvas={() => setCurrencyShow(true)}
            />
          </Container>
          <Stack className="d-none d-xl-flex" direction="horizontal" gap={4}>
            <Navlinks
              role={current_role}
              active_link="warehouse"
              notification_qty={current_nofitication_qty}
              is_sidebar={false}
            />
          </Stack>
        </Stack>
      </MenuTemplate>
      <PagePostionBar
        site_name="Warehouse"
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
          active_link="warehouse"
          notification_qty={current_nofitication_qty}
          is_sidebar={true}
        />
      </CustomSidebar>
      <CurrencyOffcanvas
        showOffcanvas={currencyShow}
        hideFunction={() => setCurrencyShow(false)}
        current_currency={currency}
      />
    </nav>
  );
}

WarehouseMenu.PropTypes = {
  current_role: PropTypes.string.isRequired,
  current_nofitication_qty: PropTypes.number.isRequired,
  is_org_switch_needed: PropTypes.bool.isRequired,
  org_view: PropTypes.bool.isRequired,
  user: PropTypes.object.isRequired,
  currency: PropTypes.string.isRequired,
};

export default WarehouseMenu;
