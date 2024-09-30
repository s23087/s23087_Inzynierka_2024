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
import getDescription from "@/utils/warehouse/get_description";
import dropdown_big_down from "../../../../public/icons/dropdown_big_down.png";
import view_outside_icon from "../../../../public/icons/view_outside_icon.png";
import view_warehouse_icon from "../../../../public/icons/view_warehouse_icon.png";
import getItemOwners from "@/utils/warehouse/get_item_owners";
function ViewItemOffcanvas({
  showOffcanvas,
  hideFunction,
  item,
  currency,
  isOrg,
}) {
  const [users, setUsers] = useState([]);
  const [choosenUser, setChoosenUser] = useState();
  const [isOurWarehouseView, setIsOurWarehouseView] = useState(true);
  const [restInfo, setRestInfo] = useState({
    outsideItemInfos: [],
    ownedItemInfos: [],
  });
  const [description, setDescription] = useState(null);
  useEffect(() => {
    if (showOffcanvas) {
      let restData = getRestInfo(currency, item.itemId, isOrg);
      restData.then((data) => setRestInfo(data));
      let desc = getDescription(item.itemId);
      desc.then((data) => setDescription(data));
    }
    if (isOrg) {
      let users = getItemOwners(item.itemId);
      users.then((data) => {
        setUsers(data);
        if (data[0]) {
          setChoosenUser(data[0].idUser);
        }
      });
    }
  }, [showOffcanvas, currency, item.itemId, isOrg]);
  const statusColorStyle = {
    color:
      getStatusColor(item.statusName) === "var(--main-yellow)"
        ? "var(--warning-color)"
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
                {isOrg && item.users.length > 0 ? (
                  <Form.Group>
                    <Form.Label className="blue-main-text">User:</Form.Label>
                    <Form.Select
                      className="input-style shadow-sm"
                      onChange={(e) => {
                        setChoosenUser(e.target.value);
                      }}
                    >
                      {Object.values(users).map((value) => {
                        return (
                          <option key={value.idUser} value={value.idUser}>
                            {value.username + " " + value.surname}
                          </option>
                        );
                      })}
                    </Form.Select>
                  </Form.Group>
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
                    {description ? description : "Loading.."}
                  </p>
                </Container>
              </Stack>
            </Col>
            <Col xs="12" md="6" lg="4" className="px-0 offset-lg-2">
              <Container
                className="pt-5 text-center overflow-x-scroll"
                key={choosenUser}
              >
                {restInfo ? (
                  <ItemTable
                    restInfo={{
                      outsideItemInfos:
                        isOrg && choosenUser
                          ? restInfo.outsideItemInfos.filter(
                              (e) => e.userId == choosenUser,
                            )
                          : restInfo.outsideItemInfos,
                      ownedItemInfos:
                        isOrg && choosenUser
                          ? restInfo.ownedItemInfos.filter(
                              (e) => e.userId == choosenUser,
                            )
                          : restInfo.ownedItemInfos,
                    }}
                    isOurWarehouse={isOurWarehouseView}
                  />
                ) : (
                  <div className="spinner-border blue-main-text text-center">
                    <span className="visually-hidden">Loading...</span>
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
