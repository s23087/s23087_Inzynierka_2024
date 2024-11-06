"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import ErrorMessage from "../smaller_components/error_message";

/**
 * Modal element that allow to add waybill to delivery.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.modalShow Modal show parameter.
 * @param {Function} props.onHideFunction Function that will close modal (set modalShow to false).
 * @param {Function} props.addAction Function that will activate after clicking add.
 * @param {Function} props.waybillExist Function that will check if new waybill does not already exist in added waybills.
 * @return {JSX.Element} Modal element
 */
function AddWaybillWindow({
  modalShow,
  onHideFunction,
  addAction,
  waybillExist,
}) {
  // True if waybill input is incorrect
  const [waybillIsInvalid, setWaybillIsInvalid] = useState(false);
  const [newWaybill, setNewWaybill] = useState("");
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
                setNewWaybill(e.target.value);
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
                    if (newWaybill === "") {
                      setWaybillIsInvalid(true);
                      return;
                    }
                    addAction(newWaybill);
                    setNewWaybill("");
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

AddWaybillWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  addAction: PropTypes.func.isRequired,
  waybillExist: PropTypes.func.isRequired,
};

export default AddWaybillWindow;
