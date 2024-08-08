import { Container, Row, Col } from "react-bootstrap";
import Image from "next/image";
import RegistrationForm from "../../../components/registration/registration_form";
import BigLogo from "../../../../public/big_logo.png";

export default function RegistrationFormHolder() {
  const formDistanceStyle = {
    "margin-top": "100px",
  };
  return (
    <main
      className="d-flex h-100 w-100 justify-content-center"
      style={formDistanceStyle}
    >
      <Container className="px-5 mb-5">
        <Row className="mb-4">
          <Col className="text-center">
            <Image src={BigLogo} alt="Logo" />
          </Col>
        </Row>
        <Row className="mb-4">
          <Col className="text-center">
            <RegistrationForm />
          </Col>
        </Row>
      </Container>
    </main>
  );
}
