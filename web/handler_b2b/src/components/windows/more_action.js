"use client";

import PropTypes from "prop-types";
import { Modal, Container, Row, Col, Stack, Button } from "react-bootstrap";
import Image from "next/image";
import CloseIcon from "../../../public/icons/close_black.png";

function MoreActionWindow({
  modalShow,
  onHideFunction,
  instanceName,
  addAction,
  selectAllOnPage,
  selectAll,
  deselectAll,
}) {
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-4">Action</h5>
            </Col>
            <Col className="d-flex justify-content-end pe-0">
              <Button variant="as-link" className="p-0">
                <Image
                  src={CloseIcon}
                  alt="Close icon"
                  onClick={onHideFunction}
                />
              </Button>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <Stack gap={2}>
            <Button
              variant="as-link"
              className="text-start ps-0"
              onClick={addAction}
            >
              Add {instanceName}
            </Button>
            <Button
              variant="as-link"
              className="text-start ps-0"
              onClick={selectAllOnPage}
            >
              Select all {instanceName}s on page
            </Button>
            <Button
              variant="as-link"
              className="text-start ps-0"
              onClick={selectAll}
            >
              Select all {instanceName}s
            </Button>
            <Button
              variant="as-link"
              className="text-start ps-0"
              onClick={deselectAll}
            >
              Deselect all {instanceName}s
            </Button>
          </Stack>
        </Container>
      </Modal.Body>
    </Modal>
  );
}

MoreActionWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  instanceName: PropTypes.string.isRequired,
  addAction: PropTypes.func.isRequired,
  selectAll: PropTypes.func.isRequired,
  selectAllOnPage: PropTypes.func.isRequired,
  deselectAll: PropTypes.func.isRequired,
  deselectAllOnPage: PropTypes.func.isRequired,
};

export default MoreActionWindow;