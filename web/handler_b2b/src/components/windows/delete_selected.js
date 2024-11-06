"use client";

import PropTypes from "prop-types";
import Image from "next/image";
import { Modal, Container, Row, Col, Button } from "react-bootstrap";
import CloseIcon from "../../../public/icons/close_black.png";
import { useState } from "react";

/**
 * Modal element that serves as confirmation on deleted selected.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.modalShow Modal show parameter.
 * @param {Function} props.onHideFunction Function that will close modal (set modalShow to false).
 * @param {Function} props.deleteItemFunc Function to active, when confirming delete.
 * @param {string} props.instanceName Name of object that will be deleted.
 * @param {boolean} props.isError True if error occurred in delete action.
 * @param {string} props.errorMessage 
 * @return {JSX.Element} Modal element
 */
function DeleteSelectedWindow({
  modalShow,
  onHideFunction,
  deleteItemFunc,
  instanceName,
  isError,
  errorMessage,
}) {
  /**
   * If action is activated true, otherwise should be false.
  */
  const [isActivated, setIsActivated] = useState(false);
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

DeleteSelectedWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  deleteItemFunc: PropTypes.func.isRequired,
  instanceName: PropTypes.string.isRequired,
  isError: PropTypes.bool.isRequired,
  errorMessage: PropTypes.string,
};

export default DeleteSelectedWindow;
