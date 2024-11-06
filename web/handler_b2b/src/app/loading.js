import { Container, Row, Col } from "react-bootstrap";

/**
 * Loading screen
 * @return {JSX.Element} className element
 */
export default function Loading() {
  const spinnerStyle = {
    width: "3.5rem",
    height: "3.5rem",
  };
  return (
    <Container
      className="d-flex flex-column h-100 w-100 justify-content-center align-items-center"
      fluid
    >
      <Row>
        <Col>
          <div className="spinner-grow blue-main-text" style={spinnerStyle}>
            <span className="visually-hidden">Loading...</span>
          </div>
        </Col>
      </Row>
      <Row className="pt-4">
        <Col>
          <p className="h4">Loading...</p>
        </Col>
      </Row>
    </Container>
  );
}
