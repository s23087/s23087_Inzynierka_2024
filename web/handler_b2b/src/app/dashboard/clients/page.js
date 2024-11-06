"use server";

import PropTypes from "prop-types";
import ClientMenu from "@/components/menu/wholeMenu/clients_menu";
import ClientsList from "@/components/object_list/client_list";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_notification_counter";
import getClients from "@/utils/clients/get_clients";
import getSearchClients from "@/utils/clients/get_clients_search";
import getOrgView from "@/utils/auth/get_org_view";

/**
 * Client page
 * @param {Object} props
 * @param {Object} props.searchParams Object that gives access to query parameters
 * @param {string} props.searchParams.page Value that determines on which page user is in
 * @param {string} props.searchParams.pagination Value that determines page pagination
 * @param {string} props.searchParams.isOrg Value that determines if org view is enabled
 * @param {string} props.searchParams.searchQuery Filled with value searched in objects
 * @param {string} props.searchParams.orderBy Sort value
 * @param {string} props.searchParams.country Country filter
 */
async function ClientsPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_notification_qty = await getNotificationCounter();
  const is_org_switch_needed = current_role == "Admin";
  let currentSort = searchParams.orderBy ?? ".None";
  let country = searchParams.country;
  let filterActivated = searchParams.orderBy || country;
  // Checks if org view has been enabled
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
  // Check if current role can access the page with org view enabled
  let org_view = getOrgView(current_role, orgActivated === "true");
  let isSearchTrue =
    searchParams.searchQuery !== undefined && searchParams.searchQuery !== "";
  // Download
  let clients = isSearchTrue
    ? await getSearchClients(
        org_view,
        searchParams.searchQuery,
        currentSort,
        country,
      )
    : await getClients(org_view, currentSort, country);
  // Pagination, default 10
  let clientsLength = clients ? clients.length : 0;
  let maxInstanceOnPage = searchParams.pagination ? searchParams.pagination : 10;
  let pageQty = Math.ceil(clientsLength / maxInstanceOnPage);
  pageQty = pageQty === 0 ? 1 : pageQty;
  let currentPage = parseInt(searchParams.page)
    ? parseInt(searchParams.page)
    : 1;
  let clientStart = currentPage * maxInstanceOnPage - maxInstanceOnPage;
  clientStart = clientStart < 0 ? 0 : clientStart;
  let clientEnd = clientStart + maxInstanceOnPage;

  return (
    <main className="d-flex flex-column h-100">
      <ClientMenu
        current_role={current_role}
        current_notification_qty={current_notification_qty}
        is_org_switch_needed={is_org_switch_needed}
        org_view={org_view}
        user={userInfo}
      />

      <section className="h-100 z-0">
        <ClientsList
          clients={clients}
          orgView={org_view}
          clientsEnd={clientEnd}
          clientsStart={clientStart}
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

ClientsPage.propTypes = {
  searchParams: PropTypes.object,
};

export default ClientsPage;
