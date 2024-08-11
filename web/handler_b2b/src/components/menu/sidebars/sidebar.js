import PropTypes from "prop-types";
import { Button, Offcanvas, Container, Row, Col, Stack } from "react-bootstrap";
import Image from "next/image";
import close_button from "../../../../public/icons/close_black.png";
import user_settings_icon from "../../../../public/icons/user_settings_icon.png";

function CustomSidebar({
  user_name,
  org_name,
  children,
  offcanvasShow,
  onHideAction,
}) {
  const sidebarStyle = {
    width: "281px",
  };
  const buttonStyle = {
    width: "214px",
  };
  return (
    <Offcanvas
      placement="end"
      show={offcanvasShow}
      onHide={onHideAction}
      style={sidebarStyle}
    >
      <Offcanvas.Header className="pt-1 pb-0">
        <Container>
          <Row className="d-flex justify-content-end">
            <Button
              variant="as-link"
              onClick={onHideAction}
              className="pe-0 w-auto"
            >
              <Image src={close_button} alt="close button" />
            </Button>
          </Row>
          <Row>
            <Col xs="auto">
              <Image src={user_settings_icon} alt="User icon" />
            </Col>
            <Col className="d-flex align-items-center">
              <p className="mb-0">{user_name}</p>
            </Col>
          </Row>
          <Row className="mt-3">
            <p>{org_name}</p>
          </Row>
        </Container>
      </Offcanvas.Header>

      <Offcanvas.Body>
        <Container className="h-100">
          <Row className="h-100">
            <Col xs="12" className="align-self-start">
              <Stack gap={4}>{children}</Stack>
            </Col>
            <Col xs="12" className="align-self-end pb-5">
              <Button variant="mainBlue" style={buttonStyle}>
                Log out
              </Button>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

CustomSidebar.PropTypes = {
  user_name: PropTypes.string.isRequired,
  org_name: PropTypes.string,
  children: PropTypes.object.isRequired, // All links of Navbar as horizontal stack
  offcanvasShow: PropTypes.bool.isRequired,
  onHideAction: PropTypes.func.isRequired,
};

export default CustomSidebar;
