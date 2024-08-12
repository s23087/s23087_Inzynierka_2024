import PropTypes from "prop-types";
import Link from "next/link";
import { Container } from "react-bootstrap";
import NotificationBadge from "../smaller_components/notification_icon";

function Navlinks({ role, active_link, notification_qty, is_sidebar }) {
  const role_link_dic = {
    admin: {
      Warehouse: "/dashboard/warehouse",
      Pricelist: "/dashboard/pricelist",
      Proformas: "/dashboard/proformas",
      Clients: "/dashboard/clients",
      Invoices: "/dashboard/invoices",
      Deliveries: "/dashboard/deliveries",
      Roles: "/dashboard/roles",
      Notifications: "/dashboard/notifications",
      Settings: "/dashboard/settings",
    },
    accountant: {
      Warehouse: "/dashboard/warehouse",
      Proformas: "/dashboard/proformas",
      Clients: "/dashboard/clients",
      Invoices: "/dashboard/invoices",
      Deliveries: "/dashboard/deliveries",
      Notifications: "/dashboard/notifications",
      Settings: "/dashboard/settings",
    },
    merchant: {
      Warehouse: "/dashboard/warehouse",
      Pricelist: "/dashboard/pricelist",
      Proformas: "/dashboard/proformas",
      Clients: "/dashboard/clients",
      Invoices: "/dashboard/invoices",
      Deliveries: "/dashboard/deliveries",
      Notifications: "/dashboard/notifications",
      Settings: "/dashboard/settings",
    },
    "warehouse manager": {
      Deliveries: "/dashboard/deliveries",
      Notifications: "/dashboard/notifications",
      Settings: "/dashboard/settings",
    },
    solo: {
      Warehouse: "/dashboard/warehouse",
      Pricelist: "/dashboard/pricelist",
      Proformas: "/dashboard/proformas",
      Clients: "/dashboard/clients",
      Invoices: "/dashboard/invocies",
      Deliveries: "/dashboard/deliveries",
      Notifications: "/dashboard/notifications",
      Settings: "/dashboard/settings",
    },
  };

  return (
    <>
      {Object.entries(role_link_dic[role.toLowerCase()]).map(([key, value]) =>
        active_link.toLowerCase() === key.toLowerCase() ? (
          <Container className="d-flex p-0" fluid>
            <p className="mb-0 blue-sec-text fst-italic pe-2" key={key}>
              {key === "Notifications" || key === "Settings" ? "" : key}
            </p>
            {key === "Notifications" ? (
              <Container className="position-relative d-xl-none">
                <NotificationBadge
                  qty={notification_qty}
                  top_value="0.5px"
                  right_value="0"
                />
              </Container>
            ) : null}
          </Container>
        ) : !is_sidebar &&
          (key === "Notifications" || key === "Settings") ? null : (
          <Container className="d-flex ps-0" key={key}>
            <Link href={value} className="blue-main-text text-decoration-none">
              {key}
            </Link>
            {key === "Notifications" ? (
              <Container className="position-relative d-xl-none">
                <NotificationBadge
                  qty={notification_qty}
                  top_value="0.5px"
                  right_value="0"
                />
              </Container>
            ) : null}
          </Container>
        ),
      )}
    </>
  );
}

Navlinks.PropTypes = {
  role: PropTypes.string.isRequired, // Admin, Merchant, Accountatn, Warehouse manager, Solo
  active_link: PropTypes.string.isRequired, // Name of active link from role_link_dic
  notification_qty: PropTypes.number.isRequired,
};

export default Navlinks;
