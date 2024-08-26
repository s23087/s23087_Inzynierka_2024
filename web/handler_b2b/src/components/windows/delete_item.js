"use client";

import PropTypes from "prop-types";
import Image from "next/image";
import { Modal, Container, Row, Col, Button } from "react-bootstrap";
import CloseIcon from "../../../public/icons/close_black.png";

function DeleteItemWindow({
  modalShow,
  onHideFunction,
  deleteItemFunc,
  instanceName,
  instanceId,
  isError,
}) {
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-4">Delete item</h5>
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
              <p className="text-start mb-4 red-sec-text">
                Error: Could not delete this item. Check if invoice with this
                item exist.
              </p>
            ) : (
              <p className="text-start mb-4">
                Are you sure you want to delete {instanceName} with id &apos;
                {instanceId}&apos;?
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
                    onClick={deleteItemFunc}
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

DeleteItemWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  deleteItemFunc: PropTypes.func.isRequired,
  instanceName: PropTypes.string.isRequired,
  instanceId: PropTypes.number.isRequired,
  isError: PropTypes.bool.isRequired,
};

export default DeleteItemWindow;
