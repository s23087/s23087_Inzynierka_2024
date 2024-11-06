"use client";

import PropTypes from "prop-types";
import { Modal, Container, Row, Col, Stack, Button } from "react-bootstrap";
import Image from "next/image";
import CloseIcon from "../../../public/icons/close_black.png";

/**
 * Modal element that show list of actions for user to choose. List contains: 
 * create, select all on page, select all, deselect.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.modalShow Modal show parameter.
 * @param {string} props.instanceName Name of object for create action.
 * @param {Function} props.addAction Add function for create action.
 * @param {Function} props.selectAllOnPage Function that will select all object in page.
 * @param {Function} props.selectAll Function that will select all object.
 * @param {Function} props.deselectAll Function that will deselect all object.
 * @param {boolean} props.withAdd If true create action is visible, otherwise is hidden. Default value is true.
 * @return {JSX.Element} Modal element
 */
function MoreActionWindow({
  modalShow,
  onHideFunction,
  instanceName,
  addAction,
  selectAllOnPage,
  selectAll,
  deselectAll,
  withAdd = true,
}) {
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
            {withAdd ? (
              <Button
                variant="as-link"
                className="text-start ps-0"
                onClick={addAction}
              >
                Add {instanceName}
              </Button>
            ) : null}
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

MoreActionWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  instanceName: PropTypes.string.isRequired,
  addAction: PropTypes.func,
  selectAll: PropTypes.func.isRequired,
  selectAllOnPage: PropTypes.func.isRequired,
  deselectAll: PropTypes.func.isRequired,
  withAdd: PropTypes.bool,
};

export default MoreActionWindow;
