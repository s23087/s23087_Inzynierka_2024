"use client";

import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import ErrorMessage from "../smaller_components/error_message";
import getUsers from "@/utils/flexible/get_users";
import validators from "@/utils/validators/validator";

function ChangeBidningsWindow({ modalShow, onHideFunction, value, addBinding }) {
  const [isInvalid, setIsInvalid] = useState(false);
  const [users, setUsers] = useState([])
  useEffect(() => {
    if (modalShow){
        const users = getUsers()
        users.then(data => {
            setUsers(data)
        })
    }
  }, [modalShow])
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Change bindings</h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
            <Form.Group className="mb-2">
                <Form.Label className="blue-main-text">Invoice:</Form.Label>
                <p>{value.invoiceNumber}</p>
            </Form.Group>
            <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Users:</Form.Label>
                <Form.Select
                    className="input-style shadow-sm"
                    id="choosenUser"
                >
                    {users.filter(e => e.idUser !== value.userId).map(val => {
                        return (
                            <option key={val.idUser} value={val.idUser}>{val.username + " " + val.surname}</option>
                        )
                    })}
                </Form.Select>
            </Form.Group>
          <Form.Group className="mb-4">
            <Form.Label className="blue-main-text">Qty:</Form.Label>
            <ErrorMessage message={"Qty must be a number between 1 and " + value.qty} messageStatus={isInvalid} />
            <Form.Control
              className="input-style shadow-sm"
              type="number"
              defaultValue={1}
              id="newQty"
              isInvalid={isInvalid}
              onInput={(e) => {
                if (!validators.haveOnlyNumbers(e.target.value)){
                    setIsInvalid(true)
                    return
                }
                let qty = parseInt(e.target.value)
                if (qty > 0 && qty <= value.qty){
                    setIsInvalid(false)
                } else {
                    setIsInvalid(true)
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
                  disabled={users.filter(e => e.idUser !== value.userId).length <= 0 || isInvalid}
                  onClick={() => {
                    if (isInvalid){
                        return
                    }
                    let choosenUser = document.getElementById("choosenUser")
                    let qty = document.getElementById("newQty").value
                    addBinding(value, parseInt(choosenUser.value), parseInt(qty), choosenUser.options[choosenUser.selectedIndex].text)
                    onHideFunction()
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

ChangeBidningsWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  value: PropTypes.object.isRequired,
};

export default ChangeBidningsWindow;
