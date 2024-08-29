import { Container, Row, Col } from "react-bootstrap";
import Image from "next/image";
import LoginForm from "./../components/Login_form/login_form";
import bigLogo from "../../public/big_logo.png";

export default function App() {
  return (
    <main className="d-flex h-100 w-100 justify-content-center pb-5">
      <Container className="mb-5 px-5 mx-4 text-center logo-spacing">
        <Row className="mb-4">
          <Col>
            <Image src={bigLogo} alt="Logo" priority={true} />
          </Col>
        </Row>

        <Row className="d-flex justify-content-center">
          <Col className="maxFormWidth">
            <LoginForm />
          </Col>
        </Row>
      </Container>
    </main>
  );
}
