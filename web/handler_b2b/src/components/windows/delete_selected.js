"use client";

import PropTypes from "prop-types";
import Image from "next/image";
import { Modal, Container, Row, Col, Button } from "react-bootstrap";
import CloseIcon from "../../../public/icons/close_black.png";
import { useState } from "react";

function DeleteSelectedWindow({
  modalShow,
  onHideFunction,
  deleteItemFunc,
  instanceName,
  isError,
  errorMessage,
}) {
  const [isActivated, setIsActivated] = useState(false);
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col xs="auto">
              <h5 className="mb-0 mt-3">Delete selected objects</h5>
            </Col>
            <Col className="d-flex justify-content-end pe-0">
              <Button variant="as-link" className="p-0">
                <Image src={CloseIcon} alt="Close" onClick={onHideFunction} />
              </Button>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <Row>
            {isError ? (
              <p className="text-start mb-4 red-sec-text">{errorMessage}</p>
            ) : (
              <p className="text-start mb-4">
                Are you sure you want to delete all selected {instanceName}s?
              </p>
            )}
          </Row>
          <Container>
            <Row>
              {isError ? null : (
                <Col>
                  <Button
                    variant="mainBlue"
                    className="w-100"
                    disabled={isActivated}
                    onClick={async () => {
                      setIsActivated(true);
                      await deleteItemFunc();
                      setIsActivated(false);
                    }}
                  >
                    Confirm
                  </Button>
                </Col>
              )}
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

DeleteSelectedWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  deleteItemFunc: PropTypes.func.isRequired,
  instanceName: PropTypes.string.isRequired,
  isError: PropTypes.bool.isRequired,
  errorMessage: PropTypes.string,
};

export default DeleteSelectedWindow;
