import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col } from "react-bootstrap";
import ContainerButtons from "../smaller_components/container_buttons";
import user_small_icon from "../../../public/icons/user_small_icon.png";

function ProformaContainer({ proforma, is_org, selected, boolean_value }) {
  const containerBg = {
    "background-color": "var(--sec-blue)",
  };
  const spanStyle = {
    "min-width": "159px",
    "min-height": "25px",
    "align-items": "center",
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
                  className="main-grey-bg d-flex rounded-span px-2 w-100 my-1"
                  style={spanStyle}
                >
                  <p className="mb-0">{proforma.user}</p>
                </span>
              </Col>
            </Row>
          ) : null}
          <Row className="gy-2">
            <Col xs="12" className="mb-1 mb-sm-0">
              <span
                className="main-blue-bg main-text d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">{proforma.number}</p>
              </span>
            </Col>
            <Col className="pe-1" xs="auto">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Date: {proforma.date}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Transport: {proforma.transport}</p>
              </span>
            </Col>
            <Col xs="12">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                {boolean_value ? (
                  <p className="mb-0">For: {proforma.for}</p>
                ) : (
                  <p className="mb-0">Source: {proforma.source}</p>
                )}
              </span>
            </Col>
            <Col className="pe-1 mb-1 d-md-none">
              <span
                className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center"
                style={selected ? null : containerBg}
              >
                <p className="mb-0">Item Quantity:</p>
                <p className="mb-0">{proforma.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-1 d-md-none">
              <span
                className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center"
                style={selected ? null : containerBg}
              >
                <p className="mb-0">Total Value:</p>
                <p className="mb-0">
                  {proforma.total_value} {proforma.currency_name}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col md="5" lg="5" xl="4" className="d-none d-md-block">
          <Row
            className="h-100 mx-auto align-items-center"
            style={maxContainerStyle}
          >
            <Col className="pe-1 mb-2">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Item Quantity:</p>
                <p className="mb-0">{proforma.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-2">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Total Value:</p>
                <p className="mb-0">
                  {proforma.total_value} {proforma.currency_name}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col xs="12" xl="4" className="px-0 pt-3 pt-xl-2 pb-2">
          <ContainerButtons selected={selected} />
        </Col>
      </Row>
    </Container>
  );
}

ProformaContainer.PropTypes = {
  proforma: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
  boolean_value: PropTypes.bool.isRequired, // 1 for client, 0 for user
};

export default ProformaContainer;
