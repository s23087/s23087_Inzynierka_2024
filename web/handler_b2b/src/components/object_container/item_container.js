"use client";

import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col } from "react-bootstrap";
import getStatusColor from "@/utils/warehouse/get_status_color";
import ContainerButtons from "../smaller_components/container_buttons";
import user_small_icon from "../../../public/icons/user_small_icon.png";

function ItemContainer({
  item,
  currency,
  is_org,
  selectQtyAction,
  unselectQtyAction,
  deleteAction,
  viewAction,
  modifyAction,
  selected,
}) {
  let statusColor = getStatusColor(item.statusName);
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  const statusBgStyle = {
    backgroundColor: statusColor,
    color:
      statusColor === "var(--sec-red)" || statusColor === "var(--sec-grey)"
        ? "var(--text-main-color)"
        : "var(--text-black-color)",
    minWidth: "159px",
    minHeight: "25px",
    alignItems: "center",
  };
  const getShortEan = () => {
    let ean = item.eans.join(", ");
    ean = ean.substring(0, 15);
    let index = ean.lastIndexOf(",");
    if (index === -1) return ean;
    return ean.substring(0, index) + " ...";
  };
  const getShortUsers = () => {
    let users = item.users.join(", ");
    users = users.substring(0, 40);
    let index = users.lastIndexOf(",");
    if (index === -1) return users;
    return users.substring(0, index) + " ...";
  };
  const getShortSources = () => {
    let sources = item.sources.join(", ");
    sources = sources.substring(0, 15);
    let index = sources.lastIndexOf(",");
    if (index === -1) return sources;
    return sources.substring(0, index) + " ...";
  };
  return (
    <Container
      className="py-3 black-text medium-text border-bottom-grey px-0 px-xl-3"
      style={selected ? containerBg : null}
      fluid
    >
      <Row className="mx-0 mx-md-3 mx-xl-3">
        <Col xs="12" md="7" lg="7" xl="4">
          {is_org ? (
            <Row className="mb-2">
              <Col className="d-flex">
                <Image
                  src={user_small_icon}
                  alt="user small icon"
                  className="me-2 mt-1"
                />
                <span className="spanStyle main-blue-bg main-text d-flex rounded-span px-2 w-100 my-1">
                  <p className="mb-0 w-100 d-block text-turncate">
                    {item.users.length > 0 ? item.users.join(", ") : "-"}
                  </p>
                </span>
              </Col>
            </Row>
          ) : (
            <></>
          )}
          <Row className="gy-2">
            <Col className="pe-1" xs="auto">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">Id: {item.itemId}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span className=" d-flex rounded-span px-2" style={statusBgStyle}>
                <p className="mb-0">Availability: {item.statusName}</p>
              </span>
            </Col>
            <Col xs="12" className="mb-1 mb-sm-0">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">P/N: {item.partNumber}</p>
              </span>
            </Col>
            <Col className="pe-1 mb-1 d-md-none">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Quantity:</p>
                <p className="mb-0">{item.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-1 d-md-none">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Purchase price:</p>
                <p className="mb-0">
                  {item.purchasePrice === null
                    ? "-"
                    : Math.round(item.purchasePrice * 100) / 100}{" "}
                  {currency}
                </p>
              </span>
            </Col>
            <Col xs="12">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0 text-truncate d-block w-100">
                  Name: {item.itemName}
                </p>
              </span>
            </Col>
            <Col className="pe-1 d-xxl-none" xs="auto">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">
                  Source: {item.sources.length === 0 ? "-" : getShortSources()}
                </p>
              </span>
            </Col>
            <Col className="ps-1 d-xxl-none">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                Ean: {getShortEan()}
              </span>
            </Col>
          </Row>
        </Col>
        <Col md="5" lg="5" xl="4" className="d-none d-md-block">
          <Row className="maxContainerStyle h-100 mx-auto">
            <Col className="pe-1 mb-2 mt-auto">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Quantity:</p>
                <p className="mb-0">{item.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 pe-md-0 pe-xl-2 mb-2 mt-auto">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Purchase price:</p>
                <p className="mb-0">
                  {item.purchasePrice === null
                    ? "-"
                    : Math.round(item.purchasePrice * 100) / 100}{" "}
                  {currency}
                </p>
              </span>
            </Col>
            <Col xs="12">
              <Row className="d-none d-xxl-flex">
                <Col className="pe-1" xs="auto">
                  <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                    <p className="mb-0">
                      Source:{" "}
                      {item.sources.length === 0 ? "-" : getShortSources()}
                    </p>
                  </span>
                </Col>
                <Col className="ps-1">
                  <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                    Ean: {getShortEan()}
                  </span>
                </Col>
              </Row>
            </Col>
          </Row>
        </Col>
        <Col xs="12" xl="4" className="px-0 pt-3 pt-xl-2 pb-2">
          <ContainerButtons
            selected={selected}
            selectAction={() => {
              selectQtyAction();
            }}
            unselectAction={() => {
              unselectQtyAction();
            }}
            deleteAction={deleteAction}
            viewAction={viewAction}
            modifyAction={modifyAction}
          />
        </Col>
      </Row>
    </Container>
  );
}

ItemContainer.PropTypes = {
  item: PropTypes.object.isRequired,
  currency: PropTypes.string.isRequired,
  is_org: PropTypes.bool.isRequired,
  selectQtyAction: PropTypes.func,
  unselectQtyAction: PropTypes.func,
  deleteAction: PropTypes.func,
  viewAction: PropTypes.func,
  modifyAction: PropTypes.func,
  selected: PropTypes.bool.isRequired,
};

export default ItemContainer;
