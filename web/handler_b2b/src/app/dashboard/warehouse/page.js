"use server";

import WarehouseMenu from "@/components/menu/wholeMenu/warehouse_menu";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import ProductList from "@/components/object_list/product_list";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getItems from "@/utils/warehouse/get_items";
import getSearchItems from "@/utils/warehouse/get_search_items";

export default async function WarehousePage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  const is_org_switch_needed = current_role == "Admin";
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
  const getOrgView = () => {
    return (
      (current_role == "Admin" || current_role == "Accountant") &&
      orgActivated === "true"
    );
  };
  let org_view = getOrgView();
  let currency = "PLN";
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let products = isSearchTrue
    ? await getSearchItems(currency, org_view, searchParams.searchQuery)
    : await getItems(currency, org_view);
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let pageQty = Math.ceil(products.length / maxInstanceOnPage);
  pageQty = pageQty === 0 ? 1 : pageQty;
  let currentPage = parseInt(searchParams.page)
    ? parseInt(searchParams.page)
    : 1;
  let productStart = currentPage * maxInstanceOnPage - maxInstanceOnPage;
  productStart = productStart < 0 ? 0 : productStart;
  let productEnd = productStart + maxInstanceOnPage;

  return (
    <main className="d-flex flex-column h-100">
      <WarehouseMenu
        current_role={current_role}
        current_nofitication_qty={current_nofitication_qty}
        is_org_switch_needed={is_org_switch_needed}
        org_view={org_view}
        user={userInfo}
        currency={currency}
      />

      <section className="h-100 z-0">
        <ProductList
          products={products}
          orgView={org_view}
          currency={currency}
          productStart={productStart}
          productEnd={productEnd}
        />
      </section>

      <WholeFooter
        max_instance_on_page={maxInstanceOnPage}
        page_qty={pageQty}
        current_page={currentPage}
      />
    </main>
  );
}
