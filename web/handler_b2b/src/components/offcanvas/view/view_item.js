"use client";

import Image from "next/image";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import {
  Offcanvas,
  Container,
  Row,
  Col,
  Button,
  Stack,
  Form,
} from "react-bootstrap";
import getStatusColor from "@/utils/warehouse/get_status_color";
import ItemTable from "../../tables/item_table";
import getRestInfo from "@/utils/warehouse/get_rest_info";
import dropdown_big_down from "../../../../public/icons/dropdown_big_down.png";
import view_outside_icon from "../../../../public/icons/view_outside_icon.png";
import view_warehouse_icon from "../../../../public/icons/view_warehouse_icon.png";
function ViewItemOffcanvas({
  showOffcanvas,
  hideFunction,
  item,
  currency,
  isOrg,
}) {
  const [isOurWarehouseView, setIsOurWarehouseView] = useState(true);
  const [restInfo, setRestInfo] = useState({
    outsideItemInfos: [],
    ownedItemInfos: [],
    userOwnedItems: {},
    userOutsideItems: {},
  });
  useEffect(() => {
    if (showOffcanvas) {
      let restData = getRestInfo(currency, item.itemId);
      restData.then((data) => setRestInfo(data));
    }
  }, [showOffcanvas, currency, item.itemId]);
  const statusColorStyle = {
    color:
      getStatusColor(item.statusName) === "var(--sec-red)"
        ? "var(--main-red)"
        : getStatusColor(item.statusName),
    marginBottom: ".25rem",
  };
  return (
    <Offcanvas
      className="h-100 minScalableWidth"
      show={showOffcanvas}
      onHide={hideFunction}
      placement="bottom"
    >
      <Offcanvas.Header className="border-bottom-grey px-xl-5">
        <Container className="px-3" fluid>
          <Row>
            <Col xs="6" lg="9" xl="10" className="d-flex align-items-center">
              <p className="blue-main-text h4 mb-0">Id: {item.itemId}</p>
            </Col>
            <Col xs="4" lg="2" xl="1" className="ps-1 text-end">
              {restInfo.outsideItemInfos.length === 0 &&
              restInfo.ownedItemInfos.length === 0 ? null : (
                <Button
                  variant="as-link"
                  onClick={() => {
                    setIsOurWarehouseView(!isOurWarehouseView);
                  }}
                  className="ps-0"
                >
                  <Image
                    src={
                      isOurWarehouseView
                        ? view_warehouse_icon
                        : view_outside_icon
                    }
                    alt="Hide"
                  />
                </Button>
              )}
            </Col>
            <Col xs="2" lg="1" className="ps-1 text-end">
              <Button
                variant="as-link"
                onClick={() => {
                  hideFunction();
                }}
                className="ps-2"
              >
                <Image src={dropdown_big_down} alt="Hide" />
              </Button>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Header>
      <Offcanvas.Body className="px-4 px-xl-5 mx-1 mx-xl-3 pb-0" as="div">
        <Container className="p-0" fluid>
          <Row>
            <Col xs="12" md="6">
              <Stack className="pt-3" gap={3}>
                {isOrg && item.users.lenght > 0 ? (
                  <Form.Select>
                    {item.users.map((user) => {
                      <option value={user}>{user}</option>;
                    })}
                  </Form.Select>
                ) : null}
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">P/N:</p>
                  <p className="mb-1">{item.partNumber}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Name:</p>
                  <p className="mb-1">{item.itemName}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">EAN:</p>
                  <ul className="mb-1">
                    {Object.values(item.eans).map((val) => {
                      return <li key={val}>{val}</li>;
                    })}
                  </ul>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Availability:</p>
                  <p style={statusColorStyle}>{item.statusName}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Description:</p>
                  <p className="mb-1">
                    {restInfo.itemDescription
                      ? restInfo.itemDescription
                      : "Loading.."}
                  </p>
                </Container>
              </Stack>
            </Col>
            <Col xs="12" md="6" lg="4" className="px-0 offset-lg-2">
              <Container className="pt-5 text-center">
                {restInfo ? (
                  <ItemTable
                    restInfo={restInfo}
                    isOurWarehouse={isOurWarehouseView}
                  />
                ) : (
                  <div
                    class="spinner-border blue-main-text text-center"
                    role="status"
                  >
                    <span class="visually-hidden">Loading...</span>
                  </div>
                )}
              </Container>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

ViewItemOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  item: PropTypes.object.isRequired,
  currency: PropTypes.string.isRequired,
  isOrg: PropTypes.bool.isRequired,
};

export default ViewItemOffcanvas;
