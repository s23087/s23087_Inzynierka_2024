"use server";

import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import PricelistMenu from "@/components/menu/wholeMenu/pricelist_menu";
import PricelistList from "@/components/object_list/pricelist_list";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getPricelists from "@/utils/pricelist/get_pricelist";
import getSearchPricelists from "@/utils/pricelist/get_search_pricelist";

export default async function PricelistPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  let totalL = searchParams.totalL;
  let totalG = searchParams.totalG;
  let status = searchParams.status;
  let currency = searchParams.currency;
  let type = searchParams.type;
  let filterActivated = totalL || totalG || status || currency || type;
  let currentSort = searchParams.orderBy ?? ".None";
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let pricelists = isSearchTrue
    ? await getSearchPricelists(
        searchParams.searchQuery,
        currentSort,
        totalL,
        totalG,
        status,
        currency,
        type,
      )
    : await getPricelists(currentSort, totalL, totalG, status, currency, type);
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
          filterActivated={filterActivated}
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
