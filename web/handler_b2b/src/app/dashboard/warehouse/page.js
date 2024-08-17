"use client";

import { useState } from "react";
import { Container, Stack } from "react-bootstrap";
import PagePostionBar from "@/components/menu/page_position_bar";
import SearchFilterBar from "@/components/menu/search_filter_bar";
import MenuTemplate from "@/components/menu/menu_template";
import CurrencyChangeButton from "@/components/smaller_components/currency_change_button";
import CustomSidebar from "@/components/menu/sidebars/sidebar";
import Navlinks from "@/components/menu/navlinks";
import PagationFooter from "@/components/footer/pagation_footer";
import ItemContainer from "@/components/object_container/item_container";

export default function WarehousePage() {
  const [sidebarShow, setSidebarShow] = useState(false);
  const showSidebar = () => setSidebarShow(true);
  const hideSidebar = () => setSidebarShow(false);
  const current_role = "Admin";
  const current_nofitication_qty = 1;
  const is_org_switch_needed = true;
  const org_view = false;
  const tmp_item = {
    user: "<<user>>",
    id: "<<product id>>",
    availability: "In warehouse",
    partnumber: "<<product part number>>",
    qty: 3,
    purchase_price: "<<price>>",
    currency_name: "PLN",
    name: "<<product name>>",
    source: "<<source>>",
    EAN: "<<Ean>>",
  };

  return (
    <main className="d-flex flex-column h-100">
      <nav className="fixed-top main-bg">
        <MenuTemplate sidebar_action={showSidebar} user_name="<<User name>>">
          <Stack className="ps-xl-2" direction="horizontal" gap={4}>
            <Container className="mx-auto ms-xl-2 ms-xxl-0 me-xl-5 w-auto px-xl-0 px-xxl-2">
              <CurrencyChangeButton currency="PLN" />
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
        <SearchFilterBar filter_icon_bool="false" />
        <CustomSidebar
          user_name="<<User name>>"
          org_name="<<Org name>>"
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
      </nav>

      <section className="h-100">
        <Container className="p-0 middleSectionPlacement" fluid>
          <ItemContainer item={tmp_item} is_org={true} selected={false} />
          <ItemContainer item={tmp_item} is_org={false} selected={true} />
          <ItemContainer item={tmp_item} is_org={false} selected={false} />
          <ItemContainer item={tmp_item} is_org={true} selected={true} />
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
