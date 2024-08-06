"use client";

import { Container, Row, Col, Form, Button } from "react-bootstrap";
import { useRouter } from "next/navigation";

export default function LoginForm() {
  const router = useRouter();

  return (
    <Form>
      <Form.Group className="mb-3" controlId="formEmail">
        <Form.Control
          className="input-style shadow-sm"
          type="email"
          placeholder="email"
        />
      </Form.Group>

      <Form.Group className="mb-3" controlId="formPassword">
        <Form.Control
          className="input-style shadow-sm"
          type="password"
          placeholder="password"
        />
      </Form.Group>

      <Container className="text-center mt-5 px-4" fluid>
        <Row className="mb-3">
          <Col>
            <Button
              className="rounded-md shadow w-100"
              variant="mainBlue"
              type="Submit"
            >
              Sign in
            </Button>
          </Col>
        </Row>
        <Row>
          <Col>
            <Button
              className="rounded-md shadow w-100"
              variant="secBlue"
              type="Button"
              onClick={() => router.push("/registration")}
            >
              Registration
            </Button>
          </Col>
        </Row>
      </Container>
    </Form>
  );
}
