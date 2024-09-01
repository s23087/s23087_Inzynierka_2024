"use server";

import RolesMenu from "@/components/menu/wholeMenu/roles_menu";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getUserRoles from "@/utils/roles/get_users_with_roles";
import RolesList from "@/components/object_list/roles_list";
import getRoles from "@/utils/roles/get_roles";

export default async function RolesPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  const rolesToChoose = await getRoles();
  let isSearchTrue =
    searchParams.searchQuery !== undefined && searchParams.searchQuery !== "";
  let roles = isSearchTrue
    ? await getUserRoles(searchParams.searchQuery)
    : await getUserRoles();
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let pageQty = Math.ceil(roles.length / maxInstanceOnPage);
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

      <section className="h-100">
        <RolesList
          roles={roles}
          rolesStart={rolesStart}
          rolesEnd={rolesEnd}
          rolesToChoose={rolesToChoose}
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
