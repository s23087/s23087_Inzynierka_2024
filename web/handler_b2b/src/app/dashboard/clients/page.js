"use client";

import { useState } from "react";
import { Container, Stack } from "react-bootstrap";
import PagePostionBar from "@/components/menu/page_position_bar";
import SearchFilterBar from "@/components/menu/search_filter_bar";
import MenuTemplate from "@/components/menu/menu_template";
import CustomSidebar from "@/components/menu/sidebars/sidebar";
import Navlinks from "@/components/menu/navlinks";
import PagationFooter from "@/components/footer/pagation_footer";
import ClientContainer from "@/components/object_container/clients_container";

export default function ClientsPage() {
  const itemSectionStyle = {
    marginBottom: "66px",
    marginTop: "207px",
  };
  const [sidebarShow, setSidebarShow] = useState(false);
  const showSidebar = () => setSidebarShow(true);
  const hideSidebar = () => setSidebarShow(false);
  const current_role = "Admin";
  const current_nofitication_qty = 1;
  const is_org_switch_needed = true;
  const org_view = false;
  const tmp_client = {
    user: "<<user>>",
    name: "<<name>>",
    adress: "<<adress>>",
    nip: "<<nip>>",
    country: "<<country>>",
  };

  return (
    <main className="d-flex flex-column h-100">
      <nav className="fixed-top main-bg">
        <MenuTemplate sidebar_action={showSidebar} user_name="<<User name>>">
          <Stack className="ps-xl-2" direction="horizontal" gap={4}>
            <Stack className="d-none d-xl-flex" direction="horizontal" gap={4}>
              <Navlinks
                role={current_role}
                active_link="Clients"
                notification_qty={current_nofitication_qty}
                is_sidebar={false}
              />
            </Stack>
          </Stack>
        </MenuTemplate>
        <PagePostionBar
          site_name="Clients"
          with_switch={is_org_switch_needed}
          switch_bool={org_view}
        />
        <SearchFilterBar filter_icon_bool="false" />
        <CustomSidebar
          user_name="<<User name>>"
          org_name="<<Org name>>"
          offcanvasShow={sidebarShow}
          onHideAction={hideSidebar}
        >
          <Navlinks
            role={current_role}
            active_link="Clients"
            notification_qty={current_nofitication_qty}
            is_sidebar={true}
          />
        </CustomSidebar>
      </nav>

      <section className="h-100">
        <Container className="p-0" style={itemSectionStyle} fluid>
          <ClientContainer
            client={tmp_client}
            is_org={false}
            selected={false}
          />
          <ClientContainer client={tmp_client} is_org={true} selected={false} />
          <ClientContainer client={tmp_client} is_org={false} selected={true} />
          <ClientContainer client={tmp_client} is_org={true} selected={true} />
        </Container>
      </section>

      <footer className="fixed-bottom w-100">
        <PagationFooter
          max_instance_on_page={10}
          page_qty={20}
          current_page={1}
        />
      </footer>
    </main>
  );
}