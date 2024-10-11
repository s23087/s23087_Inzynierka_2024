"use server";

import ClientMenu from "@/components/menu/wholeMenu/clients_menu";
import ClientsList from "@/components/object_list/client_list";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getClients from "@/utils/clients/get_clients";
import getSearchClients from "@/utils/clients/get_clients_search";
import getOrgView from "@/utils/auth/get_org_view";

export default async function ClientsPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  const is_org_switch_needed = current_role == "Admin";
  let currentSort = searchParams.orderBy ?? ".None";
  let country = searchParams.country;
  let filterActivated = searchParams.orderBy || country;
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
  let org_view = getOrgView(current_role, orgActivated === "true");
  let isSearchTrue =
    searchParams.searchQuery !== undefined && searchParams.searchQuery !== "";
  let clients = isSearchTrue
    ? await getSearchClients(
        org_view,
        searchParams.searchQuery,
        currentSort,
        country,
      )
    : await getClients(org_view, currentSort, country);
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let pageQty = Math.ceil(clients.length / maxInstanceOnPage);
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
        current_nofitication_qty={current_nofitication_qty}
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
