"use client";

import { useState, useRef } from "react";
import { Container, Row, Col, Form, Button } from "react-bootstrap";
import { useSearchParams } from "next/navigation";
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
    "white-space": "pre",
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

  const nextStepActivator = () => {
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
            <Form>
              {/* Step 1 */}
              <Container
                className="px-0"
                style={step === 1 ? unhidden : hidden}
              >
                <Form.Group className="mb-4" controlId="formEmail">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="email"
                    placeholder="email"
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formName">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="name"
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formSurname">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="surname"
                  />
                </Form.Group>
              </Container>

              {/* Step 2 */}
              <Container
                className="px-0"
                style={step === 2 ? unhidden : hidden}
              >
                <Form.Group className="mb-4" controlId="formCompanyName">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="company name"
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formNip">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="nip"
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formStreet">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="street"
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formCity">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="city"
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formPostalCode">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="postal code"
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="formCountry">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="text"
                    placeholder="country"
                  />
                </Form.Group>
              </Container>

              {/* Step 3 */}
              <Container
                className="px-0"
                style={step === 3 ? unhidden : hidden}
              >
                <Form.Group className="mb-4" controlId="formPassword">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="password"
                    placeholder="password"
                  />
                </Form.Group>

                <Form.Group className="pb-4" controlId="formRepeatPassword">
                  <Form.Control
                    className="input-style shadow-sm"
                    type="password"
                    placeholder="repeat password"
                  />
                </Form.Group>

                <Container className="pt-1 pb-2">
                  <Row>
                    <Col className="ps-0" xs="1">
                      <Form.Check type="checkbox" id="policy_check" label="" />
                    </Col>
                    <Col className="pe-0">
                      <Button
                        variant="as-link"
                        className="small-text px-0 text-start pt-0 lh-sm fw-lighter"
                        onClick={showModal}
                      >
                        By checking, you agree to our company&apos;s privacy
                        terms. <u>Click here</u> to read more about our policy.
                      </Button>
                    </Col>
                  </Row>
                </Container>

                <Container className="px-4 mt-2">
                  <Button className="w-100" variant="mainBlue">
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
