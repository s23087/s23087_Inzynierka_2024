"use server";

import PropTypes from "prop-types";
import RolesMenu from "@/components/menu/wholeMenu/roles_menu";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getUserRoles from "@/utils/roles/get_users_with_roles";
import RolesList from "@/components/object_list/roles_list";
import getRoles from "@/utils/roles/get_roles";

async function RolesPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  const rolesToChoose = (await getRoles()) ?? [];
  let currentSort = searchParams.orderBy ?? ".None";
  let role = searchParams.role;
  let filterActivated = searchParams.orderBy || role;
  let isSearchTrue =
    searchParams.searchQuery !== undefined && searchParams.searchQuery !== "";
  let roles = isSearchTrue
    ? await getUserRoles(searchParams.searchQuery, currentSort, role)
    : await getUserRoles(null, currentSort, role);
  let rolesLength = roles ? roles.length : 0;
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
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
        current_nofitication_qty={current_nofitication_qty}
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
