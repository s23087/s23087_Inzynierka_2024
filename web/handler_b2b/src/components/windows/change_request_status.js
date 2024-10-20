"use client";

import PropTypes from "prop-types";
import Image from "next/image";
import { Modal, Container, Row, Col, Button } from "react-bootstrap";
import CloseIcon from "../../../public/icons/close_black.png";
import { useState } from "react";
import { useRouter } from "next/navigation";
import setRequestStatus from "@/utils/documents/set_request_status";

function ChangeSelectedStatusWindow({
  modalShow,
  onHideFunction,
  requestsIds,
}) {
  const router = useRouter();
  const [isError, setIsError] = useState(false);
  const [errorMessage, setErrorMessage] = useState(false);
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
              <h5 className="mb-0 mt-3">Change status</h5>
            </Col>
            <Col className="d-flex justify-content-end pe-0">
              <Button variant="as-link" className="p-0">
                <Image
                  src={CloseIcon}
                  alt="Close"
                  onClick={() => {
                    setIsError(false);
                    onHideFunction();
                  }}
                />
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
                Choose one to change selected requests status.
              </p>
            )}
          </Row>
          <Container>
            <Row>
              <Col>
                <Button
                  variant="green"
                  className="w-100"
                  disabled={isActivated || isError}
                  onClick={async () => {
                    setIsActivated(true);
                    let errorCount = 0;
                    for (let val in requestsIds) {
                      let result = await setRequestStatus(
                        val,
                        "Fulfilled",
                        "",
                      );
                      if (!result) {
                        errorCount++;
                      }
                    }
                    if (errorCount > 0) {
                      setErrorMessage(
                        `The action encounter ${errorCount} errors.`,
                      );
                      setIsError(true);
                    } else {
                      router.refresh();
                      onHideFunction();
                    }
                    setIsActivated(false);
                  }}
                >
                  Complete
                </Button>
              </Col>
              <Col>
                <Button
                  variant="red"
                  className="w-100"
                  disabled={isActivated || isError}
                  onClick={async () => {
                    setIsActivated(true);
                    let errorCount = 0;
                    for (let val in requestsIds) {
                      let result = await setRequestStatus(
                        val,
                        "Request cancelled",
                        "",
                      );
                      if (!result) {
                        errorCount++;
                      }
                    }
                    if (errorCount > 0) {
                      setErrorMessage(
                        `The action encounter ${errorCount} errors.`,
                      );
                      setIsError(true);
                    } else {
                      router.refresh();
                      onHideFunction();
                    }
                    setIsActivated(false);
                  }}
                >
                  Reject
                </Button>
              </Col>
            </Row>
          </Container>
        </Container>
      </Modal.Body>
    </Modal>
  );
}

ChangeSelectedStatusWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  requestsIds: PropTypes.array.isRequired,
};

export default ChangeSelectedStatusWindow;
