import PagePostionBar from "@/components/menu/page_position_bar";
import SearchFilterBar from "@/components/menu/search_filter_bar";

export default function warehousePage() {
  return (
    <main>
      <nav>
        <PagePostionBar site_name="Warehouse" with_switch="true" />
        <SearchFilterBar filter_icon_bool="false" />
      </nav>
    </main>
  );
}
