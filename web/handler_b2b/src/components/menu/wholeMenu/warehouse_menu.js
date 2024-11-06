"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { Stack, Container } from "react-bootstrap";
import MenuTemplate from "../menu_template";
import CurrencyChangeButton from "@/components/smaller_components/currency_change_button";
import Navlinks from "../navlinks";
import PagePositionBar from "../page_position_bar";
import CustomSidebar from "../sidebars/sidebar";
import CurrencyOffcanvas from "@/components/offcanvas/currency_offcanvas";

/**
 * Create warehouse page menu.
 * @component
 * @param {Object} props
 * @param {string} props.current_role Current user role
 * @param {Number} props.current_notification_qty Number of unread notifications
 * @param {boolean} props.is_org_switch_needed True if org view switch is needed
 * @param {boolean} props.org_view True if org view is enabled
 * @param {{username: string, surname: string, orgName: string}} props.user Object containing user information
 * @param {string} props.currency Shortcut name of chosen by user currency
 * @return {JSX.Element} nav element
 */
function WarehouseMenu({
  current_role,
  current_notification_qty,
  is_org_switch_needed,
  org_view,
  user,
  currency,
}) {
  // useState for showing sidebar offcanvas
  const [sidebarShow, setSidebarShow] = useState(false);
  const showSidebar = () => setSidebarShow(true);
  const hideSidebar = () => setSidebarShow(false);
  // useState for showing change currency offcanvas
  const [currencyShow, setCurrencyShow] = useState(false);
  return (
    <nav className="fixed-top main-bg z-1">
      <MenuTemplate
        sidebar_action={showSidebar}
        user_name={user.username + " " + user.surname}
        current_notification_qty={current_notification_qty}
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
              notification_qty={current_notification_qty}
              is_sidebar={false}
            />
          </Stack>
        </Stack>
      </MenuTemplate>
      <PagePositionBar
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
          notification_qty={current_notification_qty}
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

WarehouseMenu.propTypes = {
  current_role: PropTypes.string.isRequired,
  current_notification_qty: PropTypes.number.isRequired,
  is_org_switch_needed: PropTypes.bool.isRequired,
  org_view: PropTypes.bool.isRequired,
  user: PropTypes.object.isRequired,
  currency: PropTypes.string.isRequired,
};

export default WarehouseMenu;
