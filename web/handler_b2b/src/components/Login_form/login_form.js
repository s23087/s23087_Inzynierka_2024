"use client";

import { useState } from "react";
import { useFormState } from "react-dom";
import { Container, Row, Col, Form, Button } from "react-bootstrap";
import Link from "next/link";
import signIn from "@/utils/login/sign_in";
import ErrorMessage from "../smaller_components/error_message";
import InputValidtor from "@/utils/validators/form_validator/inputValidator";

export default function LoginForm() {
  const [state, formAction] = useFormState(signIn, {
    error: false,
    message: "",
  });
  const [emailError, setEmailError] = useState(false);
  const [companyIdError, setComapnyIdError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  return (
    <Form action={formAction} id="loginFrom">
      <ErrorMessage message={state.message} messageStatus={state.error} />
      <Form.Group className="mb-3" controlId="formCompanyId">
        <ErrorMessage
          message="Company id is invalid"
          messageStatus={companyIdError}
        />
        <Form.Control
          className="input-style shadow-sm"
          type="text"
          name="companyId"
          placeholder="company id"
          isInvalid={state.error || companyIdError}
          onInput={(e) =>
            InputValidtor.noPathCharactersValidator(
              e.target.value,
              setComapnyIdError,
            )
          }
        />
      </Form.Group>

      <Form.Group className="mb-3" controlId="formEmail">
        <ErrorMessage message="Email is invalid" messageStatus={emailError} />
        <Form.Control
          className="input-style shadow-sm"
          type="email"
          name="email"
          placeholder="email"
          onInput={(e) =>
            InputValidtor.emailValidator(e.target.value, setEmailError, 350)
          }
          maxLength={350}
          isInvalid={state.error || emailError}
        />
      </Form.Group>

      <Form.Group className="mb-3" controlId="formPassword">
        <Form.Control
          className="input-style shadow-sm"
          type="password"
          name="password"
          placeholder="password"
          isInvalid={state.error}
        />
      </Form.Group>

      <Container className="text-center mt-3 px-4 px-md-0" fluid>
        <Row className="mb-3">
          {isLoading && !state.error ? (
            <Col>
              <div className="spinner-border blue-main-text mt-4"></div>
            </Col>
          ) : (
            <>
              <Col className="mt-3 pe-md-0" xs="12" md="5">
                <Button
                  className="rounded-md shadow w-100"
                  variant="mainBlue"
                  type="Submit"
                  onClick={() => {
                    setIsLoading(true);
                    let form = document.getElementById("loginFrom");
                    form.requestSubmit();
                  }}
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
            </>
          )}
        </Row>
      </Container>
    </Form>
  );
}
