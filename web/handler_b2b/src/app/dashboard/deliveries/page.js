"use server";

import PropTypes from "prop-types";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import DeliveryMenu from "@/components/menu/wholeMenu/delivery_menu";
import DeliveryList from "@/components/object_list/delivery_list";
import getOrgView from "@/utils/auth/get_org_view";
import getRole from "@/utils/auth/get_role";
import getDeliveries from "@/utils/deliveries/get_deliveries";
import getSearchDeliveries from "@/utils/deliveries/get_search_deliveries";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";

async function DeliveriesPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  const is_org_switch_needed = current_role == "Admin";
  let currentSort = searchParams.orderBy ?? ".None";
  let params = {
    estimatedL: searchParams.estimatedL,
    estimatedG: searchParams.estimatedG,
    deliveredL: searchParams.deliveredL,
    deliveredG: searchParams.deliveredG,
    recipient: searchParams.recipient,
    status: searchParams.status,
    company: searchParams.company,
    waybill: searchParams.waybill,
  };
  let filterActivated =
    searchParams.orderBy ||
    params.estimatedL ||
    params.estimatedG ||
    params.deliveredL ||
    params.deliveredG ||
    params.recipient ||
    params.status ||
    params.company ||
    params.waybill;
  let type = searchParams.deliveryType
    ? searchParams.deliveryType
    : "Deliveries to user";
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
  let org_view = getOrgView(current_role, orgActivated === "true");
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let deliveries = isSearchTrue
    ? await getSearchDeliveries(
        org_view,
        type === "Deliveries to user",
        searchParams.searchQuery,
        currentSort,
        params,
      )
    : await getDeliveries(
        org_view,
        type === "Deliveries to user",
        currentSort,
        params,
      );
  let deliveriesLength = deliveries ? deliveries.length : 0;
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let pageQty = Math.ceil(deliveriesLength / maxInstanceOnPage);
  pageQty = pageQty === 0 ? 1 : pageQty;
  let currentPage = parseInt(searchParams.page)
    ? parseInt(searchParams.page)
    : 1;
  let deliveriesStart = currentPage * maxInstanceOnPage - maxInstanceOnPage;
  deliveriesStart = deliveriesStart < 0 ? 0 : deliveriesStart;
  let deliveriesEnd = deliveriesStart + maxInstanceOnPage;

  return (
    <main className="d-flex flex-column h-100">
      <DeliveryMenu
        current_role={current_role}
        current_nofitication_qty={current_nofitication_qty}
        is_org_switch_needed={is_org_switch_needed}
        type={type}
        org_view={org_view}
        user={userInfo}
      />

      <section className="h-100 z-0">
        <DeliveryList
          deliveries={deliveries}
          type={type}
          orgView={org_view}
          deliveriesEnd={deliveriesEnd}
          deliveriesStart={deliveriesStart}
          currentSort={currentSort}
          filterActive={filterActivated}
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

DeliveriesPage.propTypes = {
  searchParams: PropTypes.object,
};

export default DeliveriesPage;
