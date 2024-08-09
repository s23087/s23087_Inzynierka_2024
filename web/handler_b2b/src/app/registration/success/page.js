import { Container, Row, Col, Button } from "react-bootstrap";
import Image from "next/image";
import Link from "next/link";
import BigLogo from "../../../../public/big_logo.png";

export default function SuccesPage() {
  const buttonSize = {
    "max-width": "214px",
  };
  return (
    <main className="d-flex h-100 w-100 justify-content-center">
      <Container className="px-5 mb-5 logo-spacing">
        <Row className="mb-4">
          <Col className="text-center">
            <Image src={BigLogo} alt="Logo" />
          </Col>
        </Row>
        <Row className="mb-4 pt-3">
          <Col className="text-center black-text">
            <h1 className="pb-4">Success!</h1>
            <p className="fw-bolder">Your account has been created.</p>
            <p className="fw-bolder">You can login in now.</p>
          </Col>
        </Row>
        <Row className="d-flex justify-content-center">
          <Col className="px-2" style={buttonSize}>
            <Link href="/">
              <Button variant="mainBlue" className="w-100">
                Sign in
              </Button>
            </Link>
          </Col>
        </Row>
      </Container>
    </main>
  );
}
