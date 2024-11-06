import { Container, Row, Col } from "react-bootstrap";
import UserChooser from "./user_type_input";

/**
 * Return element that allow user to choose witch registration type he wants to perform.
 * @return {JSX.Element} Container element
 */
export default function RegistrationChooser() {
  return (
    <Container className="text-center">
      <Container>
        <p className="black-text fw-semibold mb-1">
          Choose preferred user type:
        </p>
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
