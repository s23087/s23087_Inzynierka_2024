import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";

function NotificationContainer({ notification, switch_read_action }) {
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  const titleStyle = {
    maxWidth: "369px",
  };
  const messageStyle = {
    minWidth: "309px",
    minHeight: "94px",
    wordBreak: "break-word",
  };
  const buttonStyle = {
    minWidth: "140px",
    maxWidth: "171px",
    height: "48px",
    borderRadius: "5px",
  };

  return (
    <Container
      className="py-3 black-text medium-text border-bottom-grey px-0 px-xl-3"
      style={notification.isRead ? null : containerBg}
      fluid
    >
      <Row className="mx-0 mx-md-3 mx-xl-3">
        <Col xs="12" lg="6" xl="6">
          <Row className="gy-2 px-3 px-md-0">
            <Col xs="12" className="mb-1 mb-sm-0">
              <span
                className="spanStyle main-blue-bg main-text d-flex rounded-span px-2"
                style={titleStyle}
              >
                <p className="mb-0">
                  {notification.objectType} with referance{" "}
                  {notification.referance}
                </p>
              </span>
            </Col>
            <Col xs="12" className="mb-1 mb-sm-0">
              <span
                className="main-grey-bg d-flex rounded-span px-2 overflow-y-scroll"
                style={messageStyle}
              >
                <p className="mb-0 mt-2 w-100">{notification.info}</p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col
          xs="12"
          lg="6"
          xl="6"
          xxl="4"
          className="px-0 pt-3 pt-xl-2 pb-2 offset-xxl-2"
        >
          <Container className="h-100" fluid>
            <Row className="align-items-center justify-content-center justify-content-lg-end h-100">
              <Col className="pe-2 text-end" xs="auto">
                <Button
                  variant="mainBlue"
                  className="rounded-span w-100 p-0"
                  disabled={notification.objectType === "User"}
                  style={buttonStyle}
                >
                  Go to change
                </Button>
              </Col>
              <Col className="ps-2" xs="auto">
                <Button
                  variant={notification.isRead ? "red" : "green"}
                  className="rounded-span w-100"
                  onClick={switch_read_action}
                  style={buttonStyle}
                >
                  {notification.isRead ? "Mark as unread" : "Mark as read"}
                </Button>
              </Col>
            </Row>
          </Container>
        </Col>
      </Row>
    </Container>
  );
}

NotificationContainer.PropTypes = {
  notification: PropTypes.object.isRequired,
  switch_read_action: PropTypes.func.isRequired,
};

export default NotificationContainer;
