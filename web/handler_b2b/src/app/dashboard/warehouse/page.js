"use server";

import PropTypes from "prop-types";
import WarehouseMenu from "@/components/menu/wholeMenu/warehouse_menu";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import ProductList from "@/components/object_list/product_list";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_notification_counter";
import getItems from "@/utils/warehouse/get_items";
import getSearchItems from "@/utils/warehouse/get_search_items";
import getOrgView from "@/utils/auth/get_org_view";

/**
 * Warehouse page
 * @param {Object} props
 * @param {Object} props.searchParams Object that gives access to query parameters
 * @param {string} props.searchParams.page Value that determines on which page user is in
 * @param {string} props.searchParams.pagination Value that determines page pagination
 * @param {string} props.searchParams.isOrg Value that determines if org view is enabled
 * @param {string} props.searchParams.searchQuery Filled with value searched in objects
 * @param {string} props.searchParams.currency Value that determines currency of items
 * @param {string} props.searchParams.orderBy Sort value
 * @param {string} props.searchParams.status Status filter
 * @param {string} props.searchParams.ean Ean filter
 * @param {string} props.searchParams.qtyL Qty lower then filter
 * @param {string} props.searchParams.qtyG Qty greater then filter
 * @param {string} props.searchParams.priceL Price lower then filter
 * @param {string} props.searchParams.priceG Price greater then filter
 */
async function WarehousePage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_notification_qty = await getNotificationCounter();
  // Checks if view switch should be enabled
  const is_org_switch_needed = current_role == "Admin";
  let currentSort = searchParams.orderBy ?? ".None";
  let params = {
    status: searchParams.status,
    ean: searchParams.ean,
    qtyL: searchParams.qtyL,
    qtyG: searchParams.qtyG,
    priceL: searchParams.priceL,
    priceG: searchParams.priceG,
  };
  let filterActivated =
    searchParams.orderBy ||
    params.status ||
    params.ean ||
    params.qtyL ||
    params.qtyG ||
    params.priceL ||
    params.priceG;
  // Checks if org view has been enabled
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
  // Check if current role can access the page with org view enabled
  let org_view = getOrgView(current_role, orgActivated === "true");
  let currency = searchParams.currency ? searchParams.currency : "PLN";
  let isSearchTrue = searchParams.searchQuery !== undefined;
  // Download
  let products = isSearchTrue
    ? await getSearchItems(
        currency,
        org_view,
        searchParams.searchQuery,
        currentSort,
        params,
      )
    : await getItems(currency, org_view, currentSort, params);
  // Pagination, default 10
  let maxInstanceOnPage = searchParams.pagination
    ? searchParams.pagination
    : 10;
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
        current_notification_qty={current_notification_qty}
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

WarehousePage.propTypes = {
  searchParams: PropTypes.object,
};

export default WarehousePage;
