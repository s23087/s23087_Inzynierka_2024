"use server";

import PropTypes from "prop-types";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import PricelistMenu from "@/components/menu/wholeMenu/pricelist_menu";
import PricelistList from "@/components/object_list/pricelist_list";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getPricelists from "@/utils/pricelist/get_pricelist";
import getSearchPricelists from "@/utils/pricelist/get_search_pricelist";

async function PricelistPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  let params = {
    createdL : searchParams.createdL,
     createdG : searchParams.createdG,
     modifiedL : searchParams.modifiedL,
     modifiedG : searchParams.modifiedG,
     totalL : searchParams.totalL,
     totalG : searchParams.totalG,
     status : searchParams.status,
     currency : searchParams.currency,
     type : searchParams.type,
  }
  let filterActivated =
  searchParams.orderBy ||
    params.totalL ||
    params.totalG ||
    params.status ||
    params.currency ||
    params.type ||
    params.createdL ||
    params.createdG ||
    params.modifiedL ||
    params.modifiedG;
  let currentSort = searchParams.orderBy ?? ".None";
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let pricelists = isSearchTrue
    ? await getSearchPricelists(
        searchParams.searchQuery,
        currentSort,
        params
      )
    : await getPricelists(
        currentSort,
        params
      );
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let itemsLength = pricelists === null ? 0 : pricelists.length;
  let pageQty = Math.ceil(itemsLength / maxInstanceOnPage);
  pageQty = pageQty === 0 ? 1 : pageQty;
  let currentPage = parseInt(searchParams.page)
    ? parseInt(searchParams.page)
    : 1;
  let pricelistsStart = currentPage * maxInstanceOnPage - maxInstanceOnPage;
  pricelistsStart = pricelistsStart < 0 ? 0 : pricelistsStart;
  let pricelistsEnd = pricelistsStart + maxInstanceOnPage;

  return (
    <main className="d-flex flex-column h-100">
      <PricelistMenu
        current_role={current_role}
        current_nofitication_qty={current_nofitication_qty}
        user={userInfo}
      />

      <section className="h-100 z-0">
        <PricelistList
          pricelist={pricelists}
          pricelistStart={pricelistsStart}
          pricelistEnd={pricelistsEnd}
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

PricelistPage.propTypes = {
  searchParams: PropTypes.object
}

export default PricelistPage
