"use client";

import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import ErrorMessage from "../smaller_components/error_message";
import getUsers from "@/utils/flexible/get_users";
import validators from "@/utils/validators/validator";

/**
 * Modal element that allow to change item bindings from chosen invoice.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.modalShow Modal show parameter.
 * @param {Function} props.onHideFunction Function that will close modal (set modalShow to false).
 * @param {{invoiceNumber: string, userId: number, qty: number}} props.value Object representing bindings.
 * @param {Function} props.addBinding Function that will activate after user decide to click "Add binding".
 * @return {JSX.Element} Modal element
 */
function ChangeBindingsWindow({
  modalShow,
  onHideFunction,
  value,
  addBinding,
}) {
  // If input is incorrect the value should be set to true, otherwise false
  const [isInvalid, setIsInvalid] = useState(false);
  // List of users
  const [users, setUsers] = useState([]);
  // True if download error occurred, otherwise false
  const [userDownloadError, setUserDownloadError] = useState(false);
  useEffect(() => {
    if (modalShow) {
      getUsers().then((data) => {
        if (data === null) {
          setUserDownloadError(true);
        } else {
          setUserDownloadError(false);
          setUsers(data);
        }
      });
    }
  }, [modalShow]);
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
              <h5 className="mb-0 mt-3">Change bindings</h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <ErrorMessage
            message="Could not download users."
            messageStatus={userDownloadError}
          />
          <Form.Group className="mb-2">
            <Form.Label className="blue-main-text">Invoice:</Form.Label>
            <p>{value.invoiceNumber}</p>
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Label className="blue-main-text">Users:</Form.Label>
            <Form.Select className="input-style shadow-sm" id="chosenUser">
              {users
                .filter((e) => e.idUser !== value.userId)
                .map((val) => {
                  return (
                    <option key={val.idUser} value={val.idUser}>
                      {val.username + " " + val.surname}
                    </option>
                  );
                })}
            </Form.Select>
          </Form.Group>
          <Form.Group className="mb-4">
            <Form.Label className="blue-main-text">Qty:</Form.Label>
            <ErrorMessage
              message={"Qty must be a number between 1 and " + value.qty}
              messageStatus={isInvalid}
            />
            <Form.Control
              className="input-style shadow-sm"
              type="number"
              defaultValue={1}
              id="newQty"
              isInvalid={isInvalid}
              onInput={(e) => {
                // check if input is number and checks if input does not exceed item qty
                if (!validators.haveOnlyNumbers(e.target.value)) {
                  setIsInvalid(true);
                  return;
                }
                let qty = parseInt(e.target.value);
                if (qty > 0 && qty <= value.qty) {
                  setIsInvalid(false);
                } else {
                  setIsInvalid(true);
                }
              }}
            />
          </Form.Group>
          <Container className="pt-2">
            <Row>
              <Col>
                <Button
                  variant="mainBlue"
                  className="w-100"
                  disabled={
                    users.filter((e) => e.idUser !== value.userId).length <=
                      0 ||
                    isInvalid ||
                    userDownloadError
                  }
                  onClick={() => {
                    if (isInvalid) {
                      return;
                    }
                    let chosenUser = document.getElementById("chosenUser");
                    let qty = document.getElementById("newQty").value;
                    addBinding(
                      value,
                      parseInt(chosenUser.value),
                      parseInt(qty),
                      chosenUser.options[chosenUser.selectedIndex].text,
                    );
                    onHideFunction();
                  }}
                >
                  Add binding
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

ChangeBindingsWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  value: PropTypes.object.isRequired,
  addBinding: PropTypes.func.isRequired,
};

export default ChangeBindingsWindow;
