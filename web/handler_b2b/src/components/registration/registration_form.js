import { Container, Row, Col } from "react-bootstrap";
import UserChooser from "./user_type_input";

export default function RegistrationForm() {
  return (
    <Container className="text-center">
      <Container>
        <p className="black-text fw-semibold">Choose preferred user type:</p>
      </Container>
      <Container>
        <Row className="mb-4">
          <Col>
            <UserChooser is_org={false} />
          </Col>
        </Row>
        <Row>
          <Col>
            <UserChooser is_org={true} />
          </Col>
        </Row>
      </Container>
    </Container>
  );
}
