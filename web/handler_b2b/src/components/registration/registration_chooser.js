import { Container, Row, Col } from "react-bootstrap";
import UserChooser from "./user_type_input";

export default function RegistrationChooser() {
  return (
    <Container className="text-center">
      <Container>
        <p className="black-text fw-semibold">Choose preferred user type:</p>
      </Container>
      <Container>
        <Row className="mb-4 justify-content-around">
          <Col className="mt-4 text-md-end">
            <UserChooser is_org={false} link="/registration/form" />
          </Col>
          <Col className="mt-4 text-md-start">
            <UserChooser is_org={true} link="/registration/form" />
          </Col>
        </Row>
      </Container>
    </Container>
  );
}
