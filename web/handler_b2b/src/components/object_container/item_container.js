import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col, Button } from "react-bootstrap";
import user_small_icon from "../../../public/icons/user_small_icon.png";

function ItemContainer({ item, is_org, selected }) {
  const containerBg = {
    "background-color": "var(--sec-blue)",
  };
  const statusBgStyle = {
    "background-color": "var(--main-green)",
    "min-width": "159px",
    "min-height": "25px",
    "align-items": "center",
  };
  const spanStyle = {
    "min-width": "159px",
    "min-height": "25px",
    "align-items": "center",
  };
  const buttonStyle = {
    "min-width": "77px",
    "max-width": "95px",
  };
  const maxContainerStyle = {
    "max-width": "450px",
  };
  return (
    <Container
      className="py-3 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" md="7" lg="7" xl="4">
          {is_org ? (
            <Row className="mb-2">
              <Col className="d-flex">
                <Image
                  src={user_small_icon}
                  alt="user small icon"
                  className="me-2 mt-1"
                />
                <span
                  className="main-blue-bg main-text d-flex rounded-span px-2 w-100 my-1"
                  style={spanStyle}
                >
                  <p className="mb-0">{item.user}</p>
                </span>
              </Col>
            </Row>
          ) : null}
          <Row className="gy-2">
            <Col className="pe-1" xs="auto">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Id: {item.id}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span className=" d-flex rounded-span px-2" style={statusBgStyle}>
                <p className="mb-0">Availability: {item.availability}</p>
              </span>
            </Col>
            <Col xs="12" className="mb-1 mb-sm-0">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">P/N: {item.partnumber}</p>
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
                  {item.purchase_price} {item.currency_name}
                </p>
              </span>
            </Col>
            <Col xs="12">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Name: {item.name}</p>
              </span>
            </Col>
            <Col className="pe-1 d-xxl-none" xs="auto">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Source: {item.source}</p>
              </span>
            </Col>
            <Col className="ps-1 d-xxl-none">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Ean: {item.EAN}</p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col md="5" lg="5" xl="4" className="d-none d-md-block">
          <Row className="h-100 mx-auto" style={maxContainerStyle}>
            <Col className="pe-1 mb-2 mt-auto">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Quantity:</p>
                <p className="mb-0">{item.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-2 mt-auto">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Purchase price:</p>
                <p className="mb-0">
                  {item.purchase_price} {item.currency_name}
                </p>
              </span>
            </Col>
            <Col xs="12">
              <Row className="d-none d-xxl-flex">
                <Col className="pe-1" xs="auto">
                  <span
                    className="main-grey-bg d-flex rounded-span px-2"
                    style={spanStyle}
                  >
                    <p className="mb-0">Source: {item.source}</p>
                  </span>
                </Col>
                <Col className="ps-1">
                  <span
                    className="main-grey-bg d-flex rounded-span px-2"
                    style={spanStyle}
                  >
                    <p className="mb-0">Ean: {item.EAN}</p>
                  </span>
                </Col>
              </Row>
            </Col>
          </Row>
        </Col>
        <Col xs="12" xl="4" className="px-0 pt-3 pt-xl-2 pb-2">
          <Container className="h-100" fluid>
            <Row className="align-items-center justify-content-center h-100">
              <Col className="pe-2" xs="3" sm="auto">
                <Button
                  variant="mainBlue"
                  className="rounded-span w-100 p-0"
                  style={buttonStyle}
                >
                  {selected ? "Deselect" : "Select"}
                </Button>
              </Col>
              <Col className="px-2" xs="3" sm="auto">
                <Button
                  variant="red"
                  className="rounded-span w-100"
                  style={buttonStyle}
                >
                  Delete
                </Button>
              </Col>
              <Col className="px-2" xs="3" sm="auto">
                <Button
                  variant="mainBlue"
                  className="rounded-span w-100"
                  style={buttonStyle}
                >
                  View
                </Button>
              </Col>
              <Col className="ps-2" xs="3" sm="auto">
                <Button
                  variant="mainBlue"
                  className="rounded-span w-100"
                  style={buttonStyle}
                >
                  Modify
                </Button>
              </Col>
            </Row>
          </Container>
        </Col>
      </Row>
    </Container>
  );
}

ItemContainer.PropTypes = {
  item: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
};

export default ItemContainer;
