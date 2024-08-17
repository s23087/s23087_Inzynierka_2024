import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col } from "react-bootstrap";
import ContainerButtons from "@/components/smaller_components/container_buttons";
import user_small_icon from "../../../../public/icons/user_small_icon.png";

function CreditNoteContainer({ credit_note, is_org, selected, is_user_type }) {
  const systemColorMap = {
    "In system": "var(--main-green)",
    Requested: "var(--main-yellow)",
    "Not in system": "var(--sec-red)",
  };
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  const systemStyle = {
    backgroundColor: systemColorMap[credit_note.system],
    color:
      systemColorMap[credit_note.system] === "var(--sec-red)"
        ? "var(--text-main-color)"
        : "var(--text-black-color)",
    minWidth: "159px",
    minHeight: "25px",
    alignItems: "center",
    justifyContent: "center",
  };
  const spanStyle = {
    minWidth: "159px",
    minHeight: "25px",
    alignItems: "center",
  };
  const maxContainerStyle = {
    maxWidth: "450px",
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
                  <p className="mb-0">{credit_note.user}</p>
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
                <p className="mb-0">{credit_note.invoice}</p>
              </span>
            </Col>
            <Col xs="12" className="mb-1 mb-md-0 mt-1 mt-sm-2">
              <Row className="p-0">
                <Col className="pe-1" xs="auto">
                  <span
                    className="main-grey-bg d-flex rounded-span px-2"
                    style={spanStyle}
                  >
                    <p className="mb-0">Date: {credit_note.date}</p>
                  </span>
                </Col>
                <Col className="ps-1">
                  <span className="main-grey-bg d-flex rounded-span px-2">
                    <p className="mb-0">---------------------------------</p>
                  </span>
                </Col>
              </Row>
            </Col>
            <Col className="pe-1 mb-1 d-md-none">
              <span
                className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center"
                style={selected ? null : containerBg}
              >
                <p className="mb-0">Items Quantity:</p>
                <p className="mb-0">{credit_note.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-1 d-md-none">
              <span
                className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center"
                style={selected ? null : containerBg}
              >
                <p className="mb-0">Total Value:</p>
                <p className="mb-0">
                  {credit_note.total_value} {credit_note.currency_name}
                </p>
              </span>
            </Col>
            <Col xs="12">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">
                  {is_user_type ? "For" : "Source"}:{" "}
                  {is_user_type ? credit_note.for : credit_note.source}
                </p>
              </span>
            </Col>
            <Col className="pe-1 d-xxl-none" xs="auto">
              <span className="d-flex rounded-span px-2" style={systemStyle}>
                <p className="mb-0">{credit_note.system}</p>
              </span>
            </Col>
            <Col className="ps-1 d-xxl-none">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Due date: {credit_note.due_date}</p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col md="5" lg="5" xl="4" className="d-none d-md-block">
          <Row className="h-100 mx-auto" style={maxContainerStyle}>
            <Col className="pe-1 mb-2 mt-auto">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Items Quantity:</p>
                <p className="mb-0">{credit_note.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-2 mt-auto">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Total Value:</p>
                <p className="mb-0">
                  {credit_note.total_value} {credit_note.currency_name}
                </p>
              </span>
            </Col>
            <Col xs="12">
              <Row className="d-none d-xxl-flex">
                <Col className="pe-1" xs="auto">
                  <span
                    className="d-flex rounded-span px-2"
                    style={systemStyle}
                  >
                    <p className="mb-0">{credit_note.system}</p>
                  </span>
                </Col>
                <Col className="ps-1">
                  <span
                    className="main-grey-bg d-flex rounded-span px-2"
                    style={spanStyle}
                  >
                    <p className="mb-0">Due date: {credit_note.due_date}</p>
                  </span>
                </Col>
              </Row>
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

CreditNoteContainer.PropTypes = {
  item: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
  is_user_type: PropTypes.bool.isRequired, // If true is user invoice, if false is client invoice
};

export default CreditNoteContainer;
