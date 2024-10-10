"use server";

import WarehouseMenu from "@/components/menu/wholeMenu/warehouse_menu";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import ProductList from "@/components/object_list/product_list";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getItems from "@/utils/warehouse/get_items";
import getSearchItems from "@/utils/warehouse/get_search_items";
import getOrgView from "@/utils/auth/get_org_view";

export default async function WarehousePage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  const is_org_switch_needed = current_role == "Admin";
  let filterStatus = searchParams.status ?? null;
  let eanFilter = searchParams.ean ?? null;
  let currentSort = searchParams.orderBy ?? ".None";
  let qtyL = searchParams.qtyL ?? null;
  let qtyG = searchParams.qtyG ?? null;
  let priceL = searchParams.priceL ?? null;
  let priceG = searchParams.priceG ?? null;
  let filterActivated =
    searchParams.orderBy ||
    filterStatus ||
    eanFilter ||
    qtyL ||
    qtyG ||
    priceL ||
    priceG;
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
  let org_view = getOrgView(current_role, orgActivated === "true");
  let currency = searchParams.currency ? searchParams.currency : "PLN";
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let products = isSearchTrue
    ? await getSearchItems(
        currency,
        org_view,
        searchParams.searchQuery,
        currentSort,
        filterStatus,
        eanFilter,
        qtyL,
        qtyG,
        priceL,
        priceG,
      )
    : await getItems(
        currency,
        org_view,
        currentSort,
        filterStatus,
        eanFilter,
        qtyL,
        qtyG,
        priceL,
        priceG,
      );
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let productsLength = products ? products.length : 0;
  let pageQty = Math.ceil(productsLength / maxInstanceOnPage);
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
          filterActive={filterActivated}
          currentSort={currentSort}
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
