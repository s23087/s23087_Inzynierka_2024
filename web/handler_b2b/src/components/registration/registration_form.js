"use client";

import { useState, useRef } from "react";
import { Container, Row, Col, Form, Button } from "react-bootstrap";
import { useSearchParams } from "next/navigation";
import CountryOptions from "@/app/api/ui/country_options";
import validators from "@/utils/registration/registration_validator";
import PolicyOffcanvas from "./policy_offcanvas";

function RegistrationForm() {
  // Variable that tell if user choose Org option or Individual one
  const is_org = useSearchParams().get("is_org");

  // Modal variables
  const [show, setShow] = useState(false);
  const showModal = () => setShow(true);
  const hideModal = () => setShow(false);

  // Form change variables
  const [step, setStep] = useState(1);
  const spaceStyle = {
    whiteSpace: "pre",
  };
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };

  let headerText = useRef(
    is_org === "true"
      ? "Fill form for org user:"
      : "Fill form for individual user:",
  );

  // Form useState Variables
  const [emailError, setEmailError] = useState(false);
  const [nameError, setNameError] = useState(false);
  const [surnameError, setSurnameError] = useState(false);

  const [companyError, setCompanyError] = useState(false);
  const [nipError, setNipError] = useState(false);
  const [streetError, setStreetError] = useState(false);
  const [cityError, setCityError] = useState(false);
  const [postalError, setPostalError] = useState(false);

  const [passError, setPassError] = useState(false);
  const [repPassError, setRepPassError] = useState(false);
  const [policyError, setPolicyError] = useState(false);

  const nextStepActivator = () => {
    if (emailError || nameError || surnameError) {
      return;
    }
    if (
      validators.isEmpty("formEmail") ||
      validators.isEmpty("formName") ||
      validators.isEmpty("formSurname")
    ) {
      return;
    }
    if (
      step === 2 &&
      (companyError || nipError || streetError || cityError || postalError)
    )
      return;
    if (
      step === 2 &&
      (validators.isEmpty("formCompanyName") ||
        validators.isEmpty("formStreet") ||
        validators.isEmpty("formCity") ||
        validators.isEmpty("formPostalCode"))
    ) {
      return;
    }

    setStep(step + 1);

    switch (step) {
      case 1:
        headerText.current = "Fill your company info:";
        break;
      case 2:
        headerText.current = "Almost done!\nCreate strong password:";
        window.scrollTo(0, 0);
        break;
    }
  };

  return (
    <Container className="text-center">
      <Container>
        <p className="black-text fw-semibold" style={spaceStyle}>
          {headerText.current}
        </p>
      </Container>
      <Container className="maxFormWidth">
        <Row className="mb-4 justify-content-around">
          <Col className="mt-4 text-md-end">
            <Form id="regForm">
              {/* Step 1 */}
              <Container
                className="px-0"
                style={step === 1 ? unhidden : hidden}
              >
                <Form.Group
                  className="mb-4 position-relative"
                  controlId="formEmail"
                >
                  <Row>
                    <p
                      className="text-start mb-0 px-3 red-sec-text small-text"
                      style={emailError ? unhidden : hidden}
                    >
                      Invalid email.
                    </p>
                  </Row>
                  <Form.Control
                    className="input-style shadow-sm"
                    type="email"
                    placeholder="email"
                    required
                    isInvalid={emailError}
                    onInput={(event) => {
                      if (
                        validators.validate(
                          event.target.value,
                          validators.isEmail,
                          350,
                        )
                      ) {
                        setEmailError(false);
                      } else {
                        setEmailError(true);
                      }
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formName">
                  <Row>
                    <p
                      className="text-start mb-0 px-3 red-sec-text small-text"
                      style={nameError ? unhidden : hidden}
                    >
                      Must contain only letters and not be empty
                    </p>
                  </Row>
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="name"
                    required
                    isInvalid={nameError}
                    onInput={(event) => {
                      if (
                        validators.validate(
                          event.target.value,
                          validators.haveNoNumbers,
                          250,
                        )
                      ) {
                        setNameError(false);
                      } else {
                        setNameError(true);
                      }
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formSurname">
                  <Row>
                    <p
                      className="text-start mb-0 px-3 red-sec-text small-text"
                      style={surnameError ? unhidden : hidden}
                    >
                      Must contain only letters and not be empty
                    </p>
                  </Row>
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="surname"
                    required
                    isInvalid={surnameError}
                    onInput={(event) => {
                      if (
                        validators.validate(
                          event.target.value,
                          validators.haveNoNumbers,
                          250,
                        )
                      ) {
                        setSurnameError(false);
                      } else {
                        setSurnameError(true);
                      }
                    }}
                  />
                </Form.Group>
              </Container>

              {/* Step 2 */}
              <Container
                className="px-0"
                style={step === 2 ? unhidden : hidden}
              >
                <Form.Group className="mb-4" controlId="formCompanyName">
                  <Row>
                    <p
                      className="text-start mb-0 px-3 red-sec-text small-text"
                      style={companyError ? unhidden : hidden}
                    >
                      Max 50 letters and must not be empty
                    </p>
                  </Row>
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="company name"
                    required
                    isInvalid={companyError}
                    onInput={(event) => {
                      if (
                        validators.validate(
                          event.target.value,
                          validators.stringIsNotEmpty,
                          50,
                        )
                      ) {
                        setCompanyError(false);
                      } else {
                        setCompanyError(true);
                      }
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formNip">
                  <Row>
                    <p
                      className="text-start mb-0 px-3 red-sec-text small-text"
                      style={nipError ? unhidden : hidden}
                    >
                      Must contain only numbers.
                    </p>
                  </Row>
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="nip"
                    isInvalid={nipError}
                    onInput={(event) => {
                      if (validators.haveOnlyNumbers(event.target.value)) {
                        setNipError(false);
                      } else {
                        if (!validators.stringIsNotEmpty(event.target.value)) {
                          setNipError(false);
                        } else {
                          setNipError(true);
                        }
                      }
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formStreet">
                  <Row>
                    <p
                      className="text-start mb-0 px-3 red-sec-text small-text"
                      style={streetError ? unhidden : hidden}
                    >
                      Max 200 letters and must not be empty.
                    </p>
                  </Row>
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="street"
                    required
                    isInvalid={streetError}
                    onInput={(event) => {
                      if (
                        validators.validate(
                          event.target.value,
                          validators.stringIsNotEmpty,
                          200,
                        )
                      ) {
                        setStreetError(false);
                      } else {
                        setStreetError(true);
                      }
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formCity">
                  <Row>
                    <p
                      className="text-start mb-0 px-3 red-sec-text small-text"
                      style={cityError ? unhidden : hidden}
                    >
                      Max 200 letters and must not be empty.
                    </p>
                  </Row>
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="city"
                    required
                    isInvalid={cityError}
                    onInput={(event) => {
                      if (
                        validators.validate(
                          event.target.value,
                          validators.stringIsNotEmpty,
                          200,
                        )
                      ) {
                        setCityError(false);
                      } else {
                        setCityError(true);
                      }
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formPostalCode">
                  <Row>
                    <p
                      className="text-start mb-0 px-3 red-sec-text small-text"
                      style={postalError ? unhidden : hidden}
                    >
                      Max 25 characters and must not be empty.
                    </p>
                  </Row>
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="postal code"
                    required
                    isInvalid={postalError}
                    onInput={(event) => {
                      if (
                        validators.validate(
                          event.target.value,
                          validators.stringIsNotEmpty,
                          25,
                        )
                      ) {
                        setPostalError(false);
                      } else {
                        setPostalError(true);
                      }
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formCountry">
                  <Form.Select
                    id="countrySelect"
                    className="input-style shadow-sm"
                  >
                    <CountryOptions />
                  </Form.Select>
                </Form.Group>
              </Container>

              {/* Step 3 */}
              <Container
                className="px-0"
                style={step === 3 ? unhidden : hidden}
              >
                <Form.Group className="mb-4" controlId="formPassword">
                  <Row>
                    <p
                      className="text-start mb-0 px-3 red-sec-text small-text"
                      style={passError || repPassError ? unhidden : hidden}
                    >
                      Password must be the same and not empty.
                    </p>
                  </Row>
                  <Form.Control
                    className="input-style shadow-sm"
                    type="password"
                    placeholder="password"
                    required
                    isInvalid={passError}
                  />
                </Form.Group>

                <Form.Group className="pb-4" controlId="formRepeatPassword">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="password"
                    placeholder="repeat password"
                    required
                    isInvalid={repPassError}
                  />
                </Form.Group>

                <Container className="pt-1 pb-2">
                  <Form.Group>
                    <Row>
                      <Col className="ps-0" xs="1">
                        <Form.Check
                          type="checkbox"
                          id="policy_check"
                          label=""
                          onChange={(event) =>
                            setPolicyError(!event.target.value)
                          }
                          isInvalid={policyError}
                        />
                      </Col>
                      <Col className="pe-0">
                        <Button
                          variant="as-link"
                          className="small-text px-0 text-start pt-0 lh-sm fw-lighter"
                          onClick={showModal}
                        >
                          By checking, you agree to our company&apos;s privacy
                          terms. <u>Click here</u> to read more about our
                          policy.
                        </Button>
                      </Col>
                    </Row>
                    <Row>
                      <p
                        className="text-start mb-0 px-0 red-sec-text small-text"
                        style={policyError ? unhidden : hidden}
                      >
                        You must agree to our policy.
                      </p>
                    </Row>
                  </Form.Group>
                </Container>

                <Container className="px-4 mt-2">
                  <Button
                    className="w-100"
                    variant="mainBlue"
                    type="submit"
                    onClick={(event) => {
                      event.preventDefault();

                      let pass = document.getElementById("formPassword");
                      let repPass =
                        document.getElementById("formRepeatPassword");
                      let policyValue = document.getElementById("policy_check");

                      if (
                        !validators.stringAreEqual(pass.value, repPass.value) ||
                        !validators.stringIsNotEmpty(pass.value) ||
                        !validators.stringIsNotEmpty(repPass.value)
                      ) {
                        setPassError(true);
                        setRepPassError(true);
                        return;
                      }

                      setPassError(false);
                      setRepPassError(false);

                      if (!policyValue.checked) {
                        setPolicyError(true);
                        return;
                      }

                      let form = document.getElementById("regForm");
                      form.submit();
                    }}
                  >
                    Create Account
                  </Button>
                </Container>
              </Container>

              <Container
                className="text-center mt-5 px-4"
                fluid
                style={step === 3 ? hidden : unhidden}
              >
                <Row className="mb-3">
                  <Col>
                    <Button variant="as-link" onClick={nextStepActivator}>
                      &gt;&gt; Next step
                    </Button>
                  </Col>
                </Row>
              </Container>
            </Form>
          </Col>
        </Row>
      </Container>
      <Container>
        <PolicyOffcanvas show={show} hideFunction={hideModal} />
      </Container>
    </Container>
  );
}

export default RegistrationForm;
