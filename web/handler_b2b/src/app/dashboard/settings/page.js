"use client";

import { useState } from "react";
import { Container, Stack } from "react-bootstrap";
import PagePostionBar from "@/components/menu/page_position_bar";
import MenuTemplate from "@/components/menu/menu_template";
import CustomSidebar from "@/components/menu/sidebars/sidebar";
import Navlinks from "@/components/menu/navlinks";

export default function SettingsPage() {
  const itemSectionStyle = {
    marginBottom: "66px",
    marginTop: "142px",
  };
  const [sidebarShow, setSidebarShow] = useState(false);
  const showSidebar = () => setSidebarShow(true);
  const hideSidebar = () => setSidebarShow(false);
  const current_role = "Admin";
  const current_nofitication_qty = 1;

  return (
    <main className="d-flex flex-column h-100">
      <nav className="fixed-top main-bg border-bottom-grey">
        <MenuTemplate
          sidebar_action={showSidebar}
          user_name="<<User name>>"
          user_window_active_link="Settings"
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
          user_name="<<User name>>"
          org_name="<<Org name>>"
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

      <section className="h-100">
        <Container className="p-0" style={itemSectionStyle} fluid></Container>
      </section>
    </main>
  );
}
