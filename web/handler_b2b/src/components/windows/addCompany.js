"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { useFormState } from "react-dom";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import validators from "@/utils/validators/validator";
import { useRouter } from "next/navigation";
import ErrorMessage from "../smaller_components/error_message";
import createDeliveryCompany from "@/utils/deliveries/create_delivery_company";

function AddDeliveryCompanyWindow({ modalShow, onHideFunction, companies }) {
  const router = useRouter();
  const [nameError, setNameError] = useState(false);
  const [state, formAction] = useFormState(createDeliveryCompany, {
    error: false,
    completed: false,
    message: "",
  });
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Add delivery company</h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <Form id="companyAdd" action={formAction}>
            <ErrorMessage message={state.message} messageStatus={state.error} />
            <Form.Group className="mb-2">
              <Form.Label className="blue-main-text">Company name:</Form.Label>
              <Form.Control
                className="input-style shadow-sm"
                type="text"
                name="name"
                placeholder="company name"
                isInvalid={nameError}
                onInput={(e) => {
                  if (
                    validators.stringIsNotEmpty(e.target.value) &&
                    validators.lengthSmallerThen(e.target.value, 40) &&
                    !Object.values(companies).find(
                      (d) =>
                        d.name.toLowerCase() === e.target.value.toLowerCase(),
                    )
                  ) {
                    setNameError(false);
                  } else {
                    setNameError(true);
                  }
                }}
              />
              <ErrorMessage
                message="Company already exist or it's length exceed 40 chars or is empty."
                messageStatus={nameError}
              />
            </Form.Group>
          </Form>
          {state.completed && !state.error ? (
            <Container className="px-5 mt-4">
              <Button
                variant="green"
                className="w-100"
                onClick={() => {
                  onHideFunction();
                  router.refresh();
                }}
              >
                Success!
              </Button>
            </Container>
          ) : (
            <Container className="pt-4">
              <Row>
                <Col>
                  <Button
                    variant="mainBlue"
                    className="w-100"
                    disabled={nameError}
                    onClick={(e) => {
                      e.preventDefault();
                      if (nameError) return;
                      let form = document.getElementById("companyAdd");
                      form.requestSubmit();
                    }}
                  >
                    Create
                  </Button>
                </Col>
                <Col>
                  <Button
                    variant="red"
                    className="w-100"
                    onClick={() => {
                      setNameError(false);
                      onHideFunction();
                    }}
                  >
                    Cancel
                  </Button>
                </Col>
              </Row>
            </Container>
          )}
        </Container>
      </Modal.Body>
    </Modal>
  );
}

AddDeliveryCompanyWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  statuses: PropTypes.object.isRequired,
};

export default AddDeliveryCompanyWindow;
