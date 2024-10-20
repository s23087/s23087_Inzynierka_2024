"use server";

import PropTypes from "prop-types";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import ProformaMenu from "@/components/menu/wholeMenu/proforma_menu";
import ProformaList from "@/components/object_list/proforma_list";
import getOrgView from "@/utils/auth/get_org_view";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getProformas from "@/utils/proformas/get_proformas";
import getSearchProformas from "@/utils/proformas/get_search_proformas";

async function ProformasPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  const is_org_switch_needed = current_role == "Admin";
  let currentSort = searchParams.orderBy ?? ".None";
  let params = {
    qtyL : searchParams.qtyL,
   qtyG : searchParams.qtyG,
   totalL : searchParams.totalL,
   totalG : searchParams.totalG,
   dateL : searchParams.dateL,
   dateG : searchParams.dateG,
   recipient : searchParams.recipient,
   currency : searchParams.currency,
  }
  let filterActivated =
    searchParams.orderBy ||
    params.qtyL ||
    params.qtyG ||
    params.totalL ||
    params.totalG ||
    params.dateL ||
    params.dateG ||
    params.recipient ||
    params.currency;
  let type = searchParams.proformaType
    ? searchParams.proformaType
    : "Yours proformas";
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
  let org_view = getOrgView(current_role, orgActivated === "true");
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let proformas = isSearchTrue
    ? await getSearchProformas(
        org_view,
        type === "Yours proformas",
        searchParams.searchQuery,
        currentSort,
        params
      )
    : await getProformas(
        org_view,
        type === "Yours proformas",
        currentSort,
        params
      );
  let proformasLength = proformas ? proformas.length : 0;
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let pageQty = Math.ceil(proformasLength / maxInstanceOnPage);
  pageQty = pageQty === 0 ? 1 : pageQty;
  let currentPage = parseInt(searchParams.page)
    ? parseInt(searchParams.page)
    : 1;
  let proformasStart = currentPage * maxInstanceOnPage - maxInstanceOnPage;
  proformasStart = proformasStart < 0 ? 0 : proformasStart;
  let proformasEnd = proformasStart + maxInstanceOnPage;

  return (
    <main className="d-flex flex-column h-100">
      <ProformaMenu
        type={type}
        current_role={current_role}
        current_nofitication_qty={current_nofitication_qty}
        is_org_switch_needed={is_org_switch_needed}
        org_view={org_view}
        user={userInfo}
      />

      <section className="h-100 z-0">
        <ProformaList
          proformas={proformas}
          type={type}
          orgView={org_view}
          proformasStart={proformasStart}
          proformasEnd={proformasEnd}
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

ProformasPage.propTypes = {
  searchParams: PropTypes.object
}

export default ProformasPage
