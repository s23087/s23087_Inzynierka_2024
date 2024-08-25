"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import validators from "@/utils/validators/validator";

function AddEanWindow({ modalShow, onHideFunction, addAction, eanExistFun }) {
  const [isInvalid, setIsInvalid] = useState(false);
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };
  const [newEan, setNewEan] = useState("");
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Add Ean</h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <Row>
            <p
              className="text-start mb-0 px-3 red-sec-text small-text"
              style={isInvalid ? unhidden : hidden}
            >
              Ean already exist or have letters or empty
            </p>
          </Row>
          <Form.Group className="mb-4">
            <Form.Control
              className="input-style shadow-sm"
              type="text"
              placeholder="ean"
              isInvalid={isInvalid}
              onInput={(e) => {
                if (
                  validators.haveOnlyNumbers(e.target.value) &&
                  !eanExistFun(e.target.value)
                ) {
                  setIsInvalid(false);
                } else {
                  setIsInvalid(true);
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
                  onClick={() => {
                    if (isInvalid) return;
                    if (newEan === "") {
                      setIsInvalid(true);
                      return;
                    }
                    addAction(newEan);
                    setNewEan("");
                    onHideFunction();
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
        </Container>
      </Modal.Body>
    </Modal>
  );
}

AddEanWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  addAction: PropTypes.func.isRequired,
  eanExistFun: PropTypes.func.isRequired,
};

export default AddEanWindow;
