"use server";
import PropTypes from "prop-types";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import OutsideItemsMenu from "@/components/menu/wholeMenu/outside_item_menu";
import OutsideItemList from "@/components/object_list/outside_item_list";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getOutsideItems from "@/utils/outside_items/get_outside_items";
import getSearchOutsideItems from "@/utils/outside_items/get_search_outside_items";

async function OutsideItemsPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  let currentSort = searchParams.orderBy ?? ".None";
  let params = {
    qtyL: searchParams.qtyL,
    qtyG: searchParams.qtyG,
    priceL: searchParams.priceL,
    priceG: searchParams.priceG,
    source: searchParams.source,
    currency: searchParams.currency,
  };
  let filterActivated =
    searchParams.orderBy ||
    params.qtyL ||
    params.qtyG ||
    params.priceL ||
    params.priceG ||
    params.source ||
    params.currency;
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let outsideItems = isSearchTrue
    ? await getSearchOutsideItems(searchParams.searchQuery, currentSort, params)
    : await getOutsideItems(currentSort, params);
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let pageQty = Math.ceil(outsideItems.length / maxInstanceOnPage);
  pageQty = pageQty === 0 ? 1 : pageQty;
  let currentPage = parseInt(searchParams.page)
    ? parseInt(searchParams.page)
    : 1;
  let outsideItemsStart = currentPage * maxInstanceOnPage - maxInstanceOnPage;
  outsideItemsStart = outsideItemsStart < 0 ? 0 : outsideItemsStart;
  let outsideItemsEnd = outsideItemsStart + maxInstanceOnPage;
  return (
    <main className="d-flex flex-column h-100">
      <OutsideItemsMenu
        current_role={current_role}
        current_nofitication_qty={current_nofitication_qty}
        user={userInfo}
      />

      <section className="h-100 z-0">
        <OutsideItemList
          items={outsideItems}
          itemsStart={outsideItemsStart}
          itemsEnd={outsideItemsEnd}
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

OutsideItemsPage.propTypes = {
  searchParams: PropTypes.object,
};

export default OutsideItemsPage;
