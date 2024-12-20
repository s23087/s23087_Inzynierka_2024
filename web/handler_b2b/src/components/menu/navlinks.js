import PropTypes from "prop-types";
import Link from "next/link";
import { Container } from "react-bootstrap";
import NotificationBadge from "../smaller_components/notification_icon";

/**
 * Create navigation links.
 * @component
 * @param {object} props
 * @param {string} props.role Current user role
 * @param {string} props.active_link Name of current chosen link (ex. if user is in roles page then roles will be active link)
 * @param {Number} props.notification_qty Number of unread notification
 * @param {boolean} props.is_sidebar True if navlinks are used for sidebar
 * @return {JSX.Element}
 */
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
      Invoices: "/dashboard/invoices",
      Deliveries: "/dashboard/deliveries",
      Notifications: "/dashboard/notifications",
      Settings: "/dashboard/settings",
    },
  };

  return (
    <>
      {Object.entries(role_link_dic[role.toLowerCase()]).map(([key, value]) =>
        active_link.toLowerCase() === key.toLowerCase() ? (
          <Container className="d-flex p-0" fluid key={key}>
            <p className="mb-0 blue-sec-text fst-italic pe-2">
              {(key === "Notifications" || key === "Settings") && !is_sidebar
                ? ""
                : key}
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

Navlinks.propTypes = {
  role: PropTypes.string.isRequired, // Admin, Merchant, Accountant, Warehouse manager, Solo
  active_link: PropTypes.string.isRequired, // Name of active link from role_link_dic
  notification_qty: PropTypes.number.isRequired,
  is_sidebar: PropTypes.bool.isRequired,
};

export default Navlinks;
