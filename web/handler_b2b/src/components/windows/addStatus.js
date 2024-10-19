"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { useFormState } from "react-dom";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import validators from "@/utils/validators/validator";
import createStatus from "@/utils/clients/create_status";
import { useRouter } from "next/navigation";
import ErrorMessage from "../smaller_components/error_message";

function AddAvailabilityStatusWindow({ modalShow, onHideFunction, statuses }) {
  const router = useRouter();
  const [nameError, setNameError] = useState(false);
  const [daysError, setDaysError] = useState(false);
  const [state, formAction] = useFormState(createStatus, {
    error: false,
    completed: false,
    message: "",
  });
  return (
    <Modal
      size="md"
      show={modalShow}
      centered
      className="px-4 minScalableWidth"
    >
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Add Status</h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <Form id="statusAddForm" action={formAction}>
            <ErrorMessage message={state.message} messageStatus={state.error} />
            <Form.Group className="mb-4">
              <Form.Label className="blue-main-text">Name:</Form.Label>
              <Form.Control
                className="input-style shadow-sm"
                type="text"
                name="name"
                placeholder="status name"
                isInvalid={nameError}
                onInput={(e) => {
                  if (
                    validators.stringIsNotEmpty(e.target.value) &&
                    validators.lengthSmallerThen(e.target.value, 150) &&
                    !Object.values(statuses).find(
                      (d) => d.name === e.target.value,
                    )
                  ) {
                    setNameError(false);
                  } else {
                    setNameError(true);
                  }
                }}
              />
              <ErrorMessage
                message="Status already exist or it's length exceed 150 chars."
                messageStatus={nameError}
              />
            </Form.Group>
            <Form.Group className="mb-2">
              <Form.Label className="blue-main-text">Days:</Form.Label>
              <Form.Control
                className="input-style shadow-sm"
                type="text"
                name="days"
                placeholder="realization days"
                isInvalid={daysError}
                maxLength={150}
                onInput={(e) => {
                  if (validators.haveOnlyNumbers(e.target.value)) {
                    setDaysError(false);
                  } else {
                    setDaysError(true);
                  }
                }}
              />
            </Form.Group>
            <ErrorMessage
              message="Can only be numbers."
              messageStatus={daysError}
            />
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
                    onClick={(e) => {
                      e.preventDefault();
                      if (nameError || daysError) return;
                      let form = document.getElementById("statusAddForm");
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
                    onClick={onHideFunction}
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

AddAvailabilityStatusWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  statuses: PropTypes.object.isRequired,
};

export default AddAvailabilityStatusWindow;
