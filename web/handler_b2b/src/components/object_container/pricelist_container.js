import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";
import ContainerButtons from "../smaller_components/container_buttons";

function PricelistContainer({ pricelist, selected }) {
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  const statusBgStyle = {
    backgroundColor: "var(--main-green)",
    minWidth: "159px",
    minHeight: "25px",
    alignItems: "center",
  };
  const actionButtonStyle = {
    height: "48px",
    maxWidth: "369px",
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
                className="spanStyle main-grey-bg d-flex rounded-span px-2"
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
                className="spanStyle main-grey-bg d-flex rounded-span px-2"
              >
                <p className="mb-0">Name: {pricelist.name}</p>
              </span>
            </Col>
            <Col className="pe-1" xs="auto">
              <span
                className="spanStyle main-grey-bg d-flex rounded-span px-2"
              >
                <p className="mb-0">Type: {pricelist.type}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span
                className="spanStyle main-grey-bg d-flex rounded-span px-2"
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
                className="spanStyle main-blue-bg main-text d-flex rounded-span px-2"
              >
                <p className="mb-0">Products offered: {pricelist.item_qty}</p>
              </span>
              <span
                className="spanStyle main-grey-bg d-flex rounded-span px-2 mt-2"
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
          <ContainerButtons selected={selected} />
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
