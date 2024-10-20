"use server";

import PropTypes from "prop-types";
import NotifMenu from "@/components/menu/wholeMenu/notif_menu";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getNotifications from "@/utils/notifs/get_notifications";
import NotificationList from "@/components/object_list/notif_list";

async function NotificationsPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  let notifs = await getNotifications();
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let pageQty = Math.ceil(notifs.length / maxInstanceOnPage);
  pageQty = pageQty === 0 ? 1 : pageQty;
  let currentPage = parseInt(searchParams.page)
    ? parseInt(searchParams.page)
    : 1;
  let notifStart = currentPage * maxInstanceOnPage - maxInstanceOnPage;
  notifStart = notifStart < 0 ? 0 : notifStart;
  let notifsEnd = notifStart + maxInstanceOnPage;

  return (
    <main className="d-flex flex-column h-100">
      <NotifMenu
        current_role={current_role}
        current_nofitication_qty={current_nofitication_qty}
        user={userInfo}
      />

      <section className="h-100">
        <NotificationList
          notifs={notifs}
          notifStart={notifStart}
          notifEnd={notifsEnd}
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

NotificationsPage.propTypes = {
  searchParams: PropTypes.object
}

export default NotificationsPage