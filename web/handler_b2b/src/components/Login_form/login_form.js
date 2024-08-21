"use client";

import { Container, Row, Col, Form, Button } from "react-bootstrap";
import Link from "next/link";
import signIn from "@/utils/login/sign_in";

export default function LoginForm() {
  return (
    <Form action={signIn}>
      <Form.Group className="mb-3" controlId="formCompanyId">
        <Form.Control
          className="input-style shadow-sm"
          type="text"
          name="companyId"
          placeholder="company id"
        />
      </Form.Group>

      <Form.Group className="mb-3" controlId="formEmail">
        <Form.Control
          className="input-style shadow-sm"
          type="email"
          name="email"
          placeholder="email"
        />
      </Form.Group>

      <Form.Group className="mb-3" controlId="formPassword">
        <Form.Control
          className="input-style shadow-sm"
          type="password"
          name="password"
          placeholder="password"
        />
      </Form.Group>

      <Container className="text-center mt-3 px-4 px-md-0" fluid>
        <Row className="mb-3">
          <Col className="mt-3 pe-md-0" xs="12" md="5">
            <Button
              className="rounded-md shadow w-100"
              variant="mainBlue"
              type="Submit"
            >
              Sign in
            </Button>
          </Col>
          <Col className="mt-3" xs="12" md="7">
            <Link
              className="link-underline link-underline-opacity-0 text-white w-100 h-100"
              href="/registration"
            >
              <Button
                className="rounded-md shadow w-100"
                variant="secBlue"
                type="Button"
              >
                Registration
              </Button>
            </Link>
          </Col>
        </Row>
      </Container>
    </Form>
  );
}
