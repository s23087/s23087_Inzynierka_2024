import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col } from "react-bootstrap";
import ContainerButtons from "../smaller_components/container_buttons";
import user_small_icon from "../../../public/icons/user_small_icon.png";

function ClientContainer({ client, is_org, selected }) {
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  const spanStyle = {
    minWidth: "159px",
    minHeight: "25px",
    alignItems: "center",
  };
  return (
    <Container
      className="py-3 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" md="6" lg="6" xl="4">
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
                  <p className="mb-0">{client.user}</p>
                </span>
              </Col>
            </Row>
          ) : null}
          <Row className="gy-2">
            <Col xs="12">
              <span
                className="main-blue-bg main-text d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Name: {client.name}</p>
              </span>
            </Col>
            <Col xs="12">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Adress: {client.adress}</p>
              </span>
            </Col>
            <Col className="pe-1" xs="auto">
              <span
                className="main-grey-bg d-flex rounded-span px-2 d-md-none"
                style={spanStyle}
              >
                <p className="mb-0">Nip: {client.nip}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span
                className="main-grey-bg d-flex rounded-span px-2 d-md-none"
                style={spanStyle}
              >
                <p className="mb-0">Country: {client.country}</p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col md="6" lg="6" xl="4" className="d-none d-md-block ps-xl-5">
          <Row className="gy-2 h-100 align-items-end">
            <Col xs="12">
              <span
                className="main-grey-bg d-flex rounded-span px-2"
                style={spanStyle}
              >
                <p className="mb-0">Nip: {client.nip}</p>
              </span>
              <span
                className="main-grey-bg d-flex rounded-span px-2 mt-2"
                style={spanStyle}
              >
                <p className="mb-0">Country: {client.country}</p>
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

ClientContainer.PropTypes = {
  client: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
};

export default ClientContainer;
