"use server";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import SettingNavMenu from "@/components/menu/wholeMenu/settings_menu";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_notification_counter";

async function Layout({ children }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_notification_qty = await getNotificationCounter();
  return (
    <main className="d-flex flex-column h-100">
      <SettingNavMenu
        current_role={current_role}
        current_notification_qty={current_notification_qty}
        user={userInfo}
      />
      <section className="h-100">
        <Container className="p-0 middleSectionPlacement-no-footer" fluid>
          {children}
        </Container>
      </section>
    </main>
  );
}

Layout.propTypes = {
  children: PropTypes.object,
};

export default Layout;
