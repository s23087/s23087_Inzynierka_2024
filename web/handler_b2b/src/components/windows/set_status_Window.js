"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";

/**
 * Modal element that allow to change request status using option yes/no.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.modalShow Modal show parameter.
 * @param {Function} props.onHideFunction Function that will close modal (set modalShow to false).
 * @param {string} props.actionName Name of action what will modal do visible to user.
 * @param {Function} props.actionFunc Function that will change request status upon clicking yes.
 * @return {JSX.Element} Modal element
 */
function ChangeStatusWindow({
  modalShow,
  onHideFunction,
  actionName,
  actionFunc,
}) {
  // useState boolean. If true element is loading, otherwise nothing happening.
  const [isLoading, setIsLoading] = useState(false);
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
              <h5 className="mb-0 mt-3">
                Are you sure you want {actionName} request?
              </h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <Form.Group className="mb-4">
            <Form.Label className="blue-main-text">Note:</Form.Label>
            <Form.Control
              className="input-style shadow-sm"
              as="textarea"
              rows={3}
              maxLength={50}
              id="statusNote"
            />
          </Form.Group>
        </Container>
        <Container className="mt-5 mb-2">
          <Row>
            <Col>
              <Button
                variant="mainBlue"
                className="w-100"
                disabled={isLoading}
                onClick={async () => {
                  // Set loading when action is activated. After action is fulfilled, close modal, rest note element and set loading to false.
                  setIsLoading(true);
                  let note = document.getElementById("statusNote");
                  await actionFunc(note.value);
                  note.value = "";
                  onHideFunction();
                  setIsLoading(false);
                }}
              >
                Yes
              </Button>
            </Col>
            <Col>
              <Button
                variant="red"
                className="w-100"
                onClick={() => {
                  // reset note element and close modal.
                  let note = document.getElementById("statusNote");
                  note.value = "";
                  onHideFunction();
                }}
              >
                No
              </Button>
            </Col>
          </Row>
        </Container>
      </Modal.Body>
    </Modal>
  );
}

ChangeStatusWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  actionName: PropTypes.string,
  actionFunc: PropTypes.func,
};

export default ChangeStatusWindow;
