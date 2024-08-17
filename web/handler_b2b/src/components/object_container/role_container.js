import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col, Button } from "react-bootstrap";
import user_small_icon from "../../../public/icons/user_small_icon.png";

function RoleContainer({ role, selected }) {
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };

  return (
    <Container
      className="py-3 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" sm="7" md="7" lg="6" xl="5" xxl="4">
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
                <p className="mb-0">{role.user}</p>
              </span>
            </Col>
          </Row>
          <Row className="gy-2">
            <Col xs="12" className="mb-1 mb-sm-0">
              <span
                className="spanStyle main-grey-bg d-flex rounded-span px-2"
              >
                <p className="mb-0">Role: {role.role}</p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col
          xs="12"
          sm="5"
          md="5"
          lg="4"
          xl="4"
          className="px-0 pt-3 pt-xl-2 pb-2 offset-lg-3 offset-xxl-4"
        >
          <Container className="h-100" fluid>
            <Row className="align-items-center justify-content-center h-100">
              <Col className="pe-2" xs="3" sm="auto">
                <Button
                  variant="mainBlue"
                  className="basicButtonStyle rounded-span w-100 p-0"
                >
                  {selected ? "Deselect" : "Select"}
                </Button>
              </Col>
              <Col className="ps-2" xs="3" sm="auto">
                <Button
                  variant="mainBlue"
                  className="basicButtonStyle rounded-span w-100"
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

RoleContainer.PropTypes = {
  role: PropTypes.object.isRequired,
  selected: PropTypes.bool.isRequired,
};

export default RoleContainer;
