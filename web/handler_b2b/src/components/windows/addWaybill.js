"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import ErrorMessage from "../smaller_components/error_message";

function AddWaybillWindow({
  modalShow,
  onHideFunction,
  addAction,
  waybillExist,
}) {
  const [waybillIsInvalid, setWaybillIsInvalid] = useState(false);
  const [newEan, setNewEan] = useState("");
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Add Waybill</h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <Row>
            <ErrorMessage
              message="Waybill already exist or is empty"
              messageStatus={waybillIsInvalid}
            />
          </Row>
          <Form.Group className="mb-4">
            <Form.Control
              className="input-style shadow-sm"
              type="text"
              placeholder="waybill"
              isInvalid={waybillIsInvalid}
              onInput={(e) => {
                if (
                  e.target.value.length > 0 &&
                  e.target.value.length <= 40 &&
                  !waybillExist(e.target.value)
                ) {
                  setWaybillIsInvalid(false);
                } else {
                  setWaybillIsInvalid(true);
                }
                setNewEan(e.target.value);
              }}
            />
          </Form.Group>
          <Container>
            <Row>
              <Col>
                <Button
                  variant="mainBlue"
                  className="w-100"
                  disabled={waybillIsInvalid}
                  onClick={() => {
                    if (newEan === "") {
                      setWaybillIsInvalid(true);
                      return;
                    }
                    addAction(newEan);
                    setNewEan("");
                    onHideFunction();
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
                    setWaybillIsInvalid(false);
                    onHideFunction();
                  }}
                >
                  Cancel
                </Button>
              </Col>
            </Row>
          </Container>
        </Container>
      </Modal.Body>
    </Modal>
  );
}

AddWaybillWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  addAction: PropTypes.func.isRequired,
  waybillExist: PropTypes.func.isRequired,
};

export default AddWaybillWindow;
