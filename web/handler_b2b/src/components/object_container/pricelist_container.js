import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";

function PricelistContainer({ pricelist, selected }) {
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
  const actionButtonStyle = {
    height: "48px",
    "max-width": "369px",
  };
  return (
    <Container
      className="py-3 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" md="7" lg="5" xxl="3">
          <Row className="gy-2">
            <Col className="pe-1" xs="auto">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Created: {pricelist.created}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span className=" d-flex rounded-span px-2" style={statusBgStyle}>
                <p className="mb-0">Status: {pricelist.status}</p>
              </span>
            </Col>
            <Col xs="12">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Name: {pricelist.name}</p>
              </span>
            </Col>
            <Col className="pe-1" xs="auto">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Type: {pricelist.type}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Modified: {pricelist.modified}</p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col xs="12" md="5" lg="4" xxl="3" className="mt-2 mt-xl-0">
          <Row className="gy-2 h-100 align-items-center">
            <Col xs="12">
              <span
                className="main-blue-bg main-text d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Products offered: {pricelist.item_qty}</p>
              </span>
              <span
                className="main-grey-bg d-flex rounded-span px-2 mt-2"
                style={spanStyle}
              >
                <p className="mb-0 text-nowrap overflow-x-hidden">
                  Clients: {pricelist.clients.join(", ")}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col xs="12" lg="3" xxl="2" className="mt-2 mt-md-3 mt-lg-0">
          <Row className="gy-2 h-100 align-items-center text-center">
            <Col xs="12">
              <Button
                variant="mainBlue"
                className="w-100"
                style={actionButtonStyle}
              >
                action
              </Button>
            </Col>
          </Row>
        </Col>
        <Col xs="12" xxl="4" className="px-0 pt-3 pt-xl-3 pt-xxl-0 pb-2">
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

PricelistContainer.PropTypes = {
  pricelist: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
};

export default PricelistContainer;
