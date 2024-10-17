import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col, Button } from "react-bootstrap";
import user_small_icon from "../../../public/icons/user_small_icon.png";

function RoleContainer({
  role,
  selected,
  selectAction,
  unselectAction,
  modifyAction,
}) {
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };

  return (
    <Container
      className="py-3 px-4 px-xl-5 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" sm="7" lg="6" xl="8" xxl="7">
          <Row className="mb-2">
            <Col className="d-flex">
              <Image
                src={user_small_icon}
                alt="user small icon"
                className="me-2 mt-1"
              />
              <span className="spanStyle main-blue-bg main-text d-flex rounded-span px-2 w-100 my-1">
                <p className="mb-0 text-truncate d-block w-100">
                  {role.username + " " + role.surname}
                </p>
              </span>
            </Col>
          </Row>
          <Row className="gy-2">
            <Col xs="12" className="mb-1 mb-sm-0">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0 text-truncate d-block w-100">
                  Role: {role.roleName}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col
          xs="12"
          sm="5"
          lg="6"
          xl="4"
          xxl="5"
          className="px-0 pt-3 pt-xl-2 pb-2"
        >
          <Container className="h-100" fluid>
            <Row className="align-items-center justify-content-center justify-content-sm-end h-100">
              <Col className="pe-2" xs="3" sm="auto">
                <Button
                  variant="mainBlue"
                  className="basicButtonStyle rounded-span w-100 p-0"
                  onClick={selected ? unselectAction : selectAction}
                >
                  {selected ? "Deselect" : "Select"}
                </Button>
              </Col>
              <Col className="ps-2" xs="3" sm="auto">
                <Button
                  variant="mainBlue"
                  className="basicButtonStyle rounded-span w-100"
                  onClick={modifyAction}
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

RoleContainer.propTypes = {
  role: PropTypes.object.isRequired,
  selected: PropTypes.bool.isRequired,
  selectAction: PropTypes.func,
  unselectAction: PropTypes.func,
  modifyAction: PropTypes.func,
};

export default RoleContainer;
