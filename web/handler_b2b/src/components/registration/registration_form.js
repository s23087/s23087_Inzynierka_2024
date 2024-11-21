"use client";

import { useState } from "react";
import { Container, Row, Col, Form, Button } from "react-bootstrap";
import { useSearchParams } from "next/navigation";
import registerUser from "@/utils/registration/submit_registration";
import CountryOptions from "./country_options";
import validators from "@/utils/validators/validator";
import PolicyOffcanvas from "./policy_offcanvas";
import ErrorMessage from "../smaller_components/error_message";
import InputValidator from "@/utils/validators/form_validator/inputValidator";

/**
 * Return form that allow user to sign up.
 * @return {JSX.Element} Container element
 */
function RegistrationForm() {
  // Variable that tell if user choose Org option or Individual one
  const is_org = useSearchParams().get("is_org");
  const registerUserBound = registerUser.bind(null, is_org);

  // Button loading
  const [isLoading, setIsLoading] = useState(false);

  // Modal variables
  const [show, setShow] = useState(false);
  const showModal = () => setShow(true);
  const hideModal = () => setShow(false);
  const [headerText, setHeaderText] = useState(
    is_org === "true"
      ? "Fill form for org user:"
      : "Fill form for individual user:",
  );

  // Form change variables
  const [step, setStep] = useState(1);
  // Styles
  const spaceStyle = {
    whiteSpace: "pre",
  };
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };

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

  // Action that switch form parts from visible to hidden after passing a validation
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
        setHeaderText("Fill your company info:");
        break;
      case 2:
        setHeaderText("Almost done!\nCreate strong password:");
        window.scrollTo(0, 0);
        break;
    }
  };

  return (
    <Container className="text-center">
      <Container>
        <p className="black-text fw-semibold" style={spaceStyle}>
          {headerText}
        </p>
      </Container>
      <Container className="maxFormWidth">
        <Row className="mb-4 justify-content-around">
          <Col className="mt-4 text-md-end">
            <Form id="regForm" action={registerUserBound}>
              {/* Step 1 */}
              <Container
                className="px-0"
                style={step === 1 ? unhidden : hidden}
              >
                <Form.Group
                  className="mb-4 position-relative"
                  controlId="formEmail"
                >
                  <ErrorMessage
                    message="Invalid email."
                    messageStatus={emailError}
                  />
                  <Form.Control
                    className="input-style shadow-sm"
                    type="email"
                    name="email"
                    placeholder="email"
                    required
                    isInvalid={emailError}
                    maxLength={350}
                    onInput={(e) => {
                      InputValidator.emailValidator(
                        e.target.value,
                        setEmailError,
                        350,
                      );
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formName">
                  <ErrorMessage
                    message="Must contain only letters and not be empty"
                    messageStatus={nameError}
                  />
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    name="name"
                    placeholder="name"
                    required
                    isInvalid={nameError}
                    maxLength={250}
                    onInput={(e) => {
                      InputValidator.noNumberStringValidator(
                        e.target.value,
                        setNameError,
                        250,
                      );
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formSurname">
                  <ErrorMessage
                    message="Must contain only letters and not be empty"
                    messageStatus={surnameError}
                  />
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    name="surname"
                    placeholder="surname"
                    required
                    isInvalid={surnameError}
                    maxLength={250}
                    onInput={(e) => {
                      InputValidator.noNumberStringValidator(
                        e.target.value,
                        setSurnameError,
                        250,
                      );
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
                  <ErrorMessage
                    message="Max 50 letters and must not be empty"
                    messageStatus={companyError}
                  />
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    name="company"
                    placeholder="company name"
                    required
                    isInvalid={companyError}
                    maxLength={50}
                    onInput={(e) => {
                      InputValidator.normalStringValidator(
                        e.target.value,
                        setCompanyError,
                        50,
                      );
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formNip">
                  <ErrorMessage
                    message="Must contain only numbers."
                    messageStatus={nipError}
                  />
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    name="nip"
                    placeholder="nip"
                    isInvalid={nipError}
                    onInput={(e) => {
                      if (validators.haveOnlyNumbers(e.target.value)) {
                        setNipError(false);
                      } else {
                        setNipError(true);
                      }
                      if (!validators.stringIsNotEmpty(e.target.value)) {
                        setNipError(false);
                      }
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formStreet">
                  <ErrorMessage
                    message="Max 200 letters and must not be empty."
                    messageStatus={streetError}
                  />
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="street"
                    name="street"
                    required
                    isInvalid={streetError}
                    maxLength={200}
                    onInput={(e) => {
                      InputValidator.normalStringValidator(
                        e.target.value,
                        setStreetError,
                        200,
                      );
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formCity">
                  <ErrorMessage
                    message="Max 200 letters and must not be empty."
                    messageStatus={cityError}
                  />
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="city"
                    name="city"
                    required
                    isInvalid={cityError}
                    maxLength={250}
                    onInput={(e) => {
                      InputValidator.normalStringValidator(
                        e.target.value,
                        setCityError,
                        250,
                      );
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formPostalCode">
                  <ErrorMessage
                    message="Max 25 characters and must not be empty."
                    messageStatus={postalError}
                  />
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    name="postal"
                    placeholder="postal code"
                    required
                    isInvalid={postalError}
                    maxLength={25}
                    onInput={(e) => {
                      InputValidator.normalStringValidator(
                        e.target.value,
                        setPostalError,
                        25,
                      );
                    }}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formCountry">
                  <Form.Select
                    id="countrySelect"
                    className="input-style shadow-sm"
                    name="country"
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
                  <ErrorMessage
                    message="Password must be the same and not empty."
                    messageStatus={passError || repPassError}
                  />
                  <Form.Control
                    className="input-style shadow-sm"
                    type="password"
                    name="password"
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
                    <ErrorMessage
                      message="You must agree to our policy."
                      messageStatus={policyError}
                    />
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
                      setIsLoading(true);
                      form.requestSubmit();
                    }}
                  >
                    {isLoading ? (
                      <div className="spinner-border main-text"></div>
                    ) : (
                      "Create Account"
                    )}
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
