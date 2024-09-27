"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";

function ChangeStatusWindow({
  modalShow,
  onHideFunction,
  actionName,
  actionFunc,
}) {
  const [isLoading, setIsLoading] = useState(false);
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
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

ChangeStatusWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  statuses: PropTypes.object.isRequired,
};

export default ChangeStatusWindow;
