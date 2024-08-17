import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col, Stack, Button } from "react-bootstrap";
import user_small_icon from "../../../public/icons/user_small_icon.png";

function AbstractItemContainer({ abstract_item, selected }) {
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  const buttonStyle = {
    width: "90px",
    borderRadius: "15px",
  };
  return (
    <Container
      className="py-3 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" md="7" lg="7" xl="4">
          <Row className="mb-2">
            <Col className="d-flex">
              <Image
                src={user_small_icon}
                alt="user small icon"
                className="me-2 mt-1"
              />
              <span
                className="spanStyle main-blue-bg main-text d-flex rounded-span px-2 w-100 my-1"
              >
                <p className="mb-0">{abstract_item.user}</p>
              </span>
            </Col>
          </Row>
          <Row className="gy-2">
            <Col xs="12" className="mb-0">
              <span
                className="spanStyle main-grey-bg d-flex rounded-span px-2"
              >
                <p className="mb-0">P/N: {abstract_item.partnumber}</p>
              </span>
            </Col>
            <Col xs="12" className="mb-0">
              <span
                className="spanStyle main-grey-bg d-flex rounded-span px-2"
              >
                <p className="mb-0">Source: {abstract_item.source}</p>
              </span>
            </Col>
            <Col xs="12">
              <span
                className="spanStyle main-grey-bg d-flex rounded-span px-2"
              >
                <p className="mb-0">Ean: {abstract_item.EAN}</p>
              </span>
            </Col>
            <Col className="pe-1 mb-1 d-md-none">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Quantity:</p>
                <p className="mb-0">{abstract_item.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-1 d-md-none">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Purchase price:</p>
                <p className="mb-0">
                  {abstract_item.purchase_price} {abstract_item.currency_name}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col md="5" lg="5" xl="4" className="d-none d-md-block">
          <Row
            className="maxContainerStyle h-100 mx-auto align-items-center"
          >
            <Col className="pe-1 mb-2">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Quantity:</p>
                <p className="mb-0">{abstract_item.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-2">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Purchase price:</p>
                <p className="mb-0">
                  {abstract_item.purchase_price} {abstract_item.currency_name}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col
          xs="12"
          xl="4"
          className="px-0 pt-3 pt-xl-2 pb-2 d-flex align-items-center justify-content-center"
        >
          <Stack direction="horizontal">
            <Button variant="mainBlue" className="ms-auto" style={buttonStyle}>
              Select
            </Button>
            <Button variant="red" className="mx-4" style={buttonStyle}>
              Delete
            </Button>
            <Button variant="yellow" className="me-auto" style={buttonStyle}>
              Modify
            </Button>
          </Stack>
        </Col>
      </Row>
    </Container>
  );
}

AbstractItemContainer.PropTypes = {
  abstract_item: PropTypes.object.isRequired,
};

export default AbstractItemContainer;
