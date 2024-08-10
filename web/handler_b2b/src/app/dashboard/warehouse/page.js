"use client";

import { useState } from "react";
import PagePostionBar from "@/components/menu/page_position_bar";
import SearchFilterBar from "@/components/menu/search_filter_bar";
import MenuTemplate from "@/components/menu/menu_template";
import CurrencyChangeButton from "@/components/smaller_components/currency_change_button";
import CustomSidebar from "@/components/menu/sidebars/sidebar";

export default function WarehousePage() {
  const [sidebarShow, setSidebarShow] = useState(false);
  const showSidebar = () => setSidebarShow(true);
  const hideSidebar = () => setSidebarShow(false);

  return (
    <main>
      <nav>
        <MenuTemplate
          sidebar_action={showSidebar}
        >
            <CurrencyChangeButton currency="PLN" />
        </MenuTemplate>
        <PagePostionBar site_name="Warehouse" with_switch="true" />
        <SearchFilterBar filter_icon_bool="false" />
        <CustomSidebar
          user="<<user name>>"
          org_name="<<Org name>>"
          offcanvasShow={sidebarShow}
          onHideAction={hideSidebar}
        />
      </nav>
    </main>
  );
}
