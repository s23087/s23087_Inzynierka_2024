"use server";

import PropTypes from "prop-types";
import RolesMenu from "@/components/menu/wholeMenu/roles_menu";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_notification_counter";
import getUserRoles from "@/utils/roles/get_users_with_roles";
import RolesList from "@/components/object_list/roles_list";
import getRoles from "@/utils/roles/get_roles";

/**
 * Roles page
 * @param {Object} props
 * @param {Object} props.searchParams Object that gives access to query parameters
 * @param {string} props.searchParams.page Value that determines on which page user is in
 * @param {string} props.searchParams.pagination Value that determines page pagination
 * @param {string} props.searchParams.searchQuery Filled with value searched in objects
 * @param {string} props.searchParams.orderBy Sort value
 * @param {string} props.searchParams.role Role filter
 */
async function RolesPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_notification_qty = await getNotificationCounter();
  // List of current available roles
  const rolesToChoose = (await getRoles()) ?? [];
  // Filters and sort
  let currentSort = searchParams.orderBy ?? ".None";
  let role = searchParams.role;
  let filterActivated = searchParams.orderBy || role;
  let isSearchTrue =
    searchParams.searchQuery !== undefined && searchParams.searchQuery !== "";
  // download
  let roles = isSearchTrue
    ? await getUserRoles(searchParams.searchQuery, currentSort, role)
    : await getUserRoles(null, currentSort, role);
  // Pagination, default 10
  let rolesLength = roles ? roles.length : 0;
  let maxInstanceOnPage = searchParams.pagination
    ? searchParams.pagination
    : 10;
  let pageQty = Math.ceil(rolesLength / maxInstanceOnPage);
  pageQty = pageQty === 0 ? 1 : pageQty;
  let currentPage = parseInt(searchParams.page)
    ? parseInt(searchParams.page)
    : 1;
  let rolesStart = currentPage * maxInstanceOnPage - maxInstanceOnPage;
  rolesStart = rolesStart < 0 ? 0 : rolesStart;
  let rolesEnd = rolesStart + maxInstanceOnPage;

  return (
    <main className="d-flex flex-column h-100">
      <RolesMenu
        current_role={current_role}
        current_notification_qty={current_notification_qty}
        user={userInfo}
      />

      <section className="h-100 z-0">
        <RolesList
          roles={roles}
          rolesStart={rolesStart}
          rolesEnd={rolesEnd}
          rolesToChoose={rolesToChoose}
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

RolesPage.propTypes = {
  searchParams: PropTypes.object,
};

export default RolesPage;
