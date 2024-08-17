import PropTypes from "prop-types";
import Link from "next/link";
import { Button } from "react-bootstrap";

function SettingMenu({ role }) {
  const linkStyle = {
    minHeight: "45px",
  };
  const buttonStyle = {
    height: "55px",
    width: "254px",
  };
  const adminLinksMap = {
    "Manage abstract items": "abstract_items",
    "Change password": "settings/change_password",
    "Change your data": "settings/change_data",
    "Change org data": "settings/change_organization",
    "Add user": "settings/add_user",
  };
  const accountantLinksMap = {
    "Manage abstract items": "abstract_items",
    "Change password": "settings/change_password",
    "Change your data": "settings/change_data",
  };
  const merchantLinksMap = {
    "Change password": "settings/change_password",
    "Change your data": "settings/change_data",
  };
  const warehouseManagerLinksMap = {
    "Change password": "settings/change_password",
    "Change your data": "settings/change_data",
  };
  const soloLinksMap = {
    "Manage abstract items": "abstract_items",
    "Change password": "settings/change_password",
    "Change your data": "settings/change_data",
    "Change org data": "settings/change_organization",
    "Switch to org": "settings/switch_model",
  };
  switch (role) {
    case "Admin":
      return (
        <>
          {Object.entries(adminLinksMap).map(([key, value]) => {
            return (
              <Link href={value} key={key}>
                <p className="blue-main-text ps-1" style={linkStyle}>
                  {key}
                </p>
              </Link>
            );
          })}
          <Button
            variant="mainBlue"
            className="mx-auto mx-xl-0 ms-xl-1 mt-3"
            style={buttonStyle}
          >
            Download log file
          </Button>
        </>
      );
    case "Accountant":
      return (
        <>
          {Object.entries(accountantLinksMap).map(([key, value]) => {
            return (
              <Link href={value} key={key}>
                <p className="blue-main-text ps-1" style={linkStyle}>
                  {key}
                </p>
              </Link>
            );
          })}
        </>
      );
    case "Merchant":
      return (
        <>
          {Object.entries(merchantLinksMap).map(([key, value]) => {
            return (
              <Link href={value} key={key}>
                <p className="blue-main-text ps-1" style={linkStyle}>
                  {key}
                </p>
              </Link>
            );
          })}
        </>
      );
    case "Warehouse manager":
      return (
        <>
          {Object.entries(warehouseManagerLinksMap).map(([key, value]) => {
            return (
              <Link href={value} key={key}>
                <p className="blue-main-text ps-1" style={linkStyle}>
                  {key}
                </p>
              </Link>
            );
          })}
        </>
      );
    case "Solo":
      return (
        <>
          {Object.entries(soloLinksMap).map(([key, value]) => {
            return (
              <Link href={value} key={key}>
                <p className="blue-main-text ps-1" style={linkStyle}>
                  {key}
                </p>
              </Link>
            );
          })}
          <Button
            variant="mainBlue"
            className="mx-auto mx-xl-0 ms-xl-1 mt-3"
            style={buttonStyle}
          >
            Download log file
          </Button>
        </>
      );
    default:
      return (
        <h2 className="blue-main-text text-center pt-5">404: Not found</h2>
      );
  }
}

SettingMenu.PropTypes = {
  role: PropTypes.string.isRequired,
};

export default SettingMenu;
