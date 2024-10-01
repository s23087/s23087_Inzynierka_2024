"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { useFormState } from "react-dom";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import { useRouter } from "next/navigation";
import ErrorMessage from "../smaller_components/error_message";
import StringValidtor from "@/utils/validators/form_validator/stringValidator";
import createNote from "@/utils/deliveries/create_note";

function AddNoteWindow({ modalShow, onHideFunction, deliveryId, successFun }) {
  const router = useRouter();
  const [noteError, setNoteError] = useState(false);
  const [state, formAction] = useFormState(createNote.bind(null, deliveryId), {
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
              <h5 className="mb-0 mt-3">Add Note</h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <Form id="noteAdd" action={formAction}>
            <Form.Group className="mb-4">
              <ErrorMessage
                message={state.message}
                messageStatus={state.error}
              />
              <Form.Label className="blue-main-text">Note:</Form.Label>
              <Form.Control
                className="input-style shadow-sm"
                type="text"
                as="textarea"
                rows={5}
                name="note"
                maxLength={500}
                isInvalid={noteError}
                onInput={(e) => {
                  StringValidtor.normalStringValidtor(
                    e.target.value,
                    setNoteError,
                    500,
                  );
                }}
              />
              <ErrorMessage
                message="Cannot be empty."
                messageStatus={noteError}
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
                  successFun();
                }}
              >
                Success!
              </Button>
            </Container>
          ) : (
            <Container className="pt-2">
              <Row>
                <Col>
                  <Button
                    variant="mainBlue"
                    className="w-100"
                    disabled={noteError}
                    onClick={(e) => {
                      e.preventDefault();
                      if (noteError) return;
                      let form = document.getElementById("noteAdd");
                      form.requestSubmit();
                    }}
                  >
                    Add
                  </Button>
                </Col>
                <Col>
                  <Button
                    variant="red"
                    className="w-100"
                    onClick={() => {
                      setNoteError(false);
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

AddNoteWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  deliveryId: PropTypes.number.isRequired,
  successFun: PropTypes.func.isRequired,
};

export default AddNoteWindow;
