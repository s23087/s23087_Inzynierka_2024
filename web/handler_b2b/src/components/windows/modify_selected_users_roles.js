"use client";

import PropTypes from "prop-types";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import modifyUserRole from "@/utils/roles/modify_user_role";
import { useRouter } from "next/navigation";
import { useState } from "react";
import ErrorMessage from "../smaller_components/error_message";

function ModifySelectedUserRole({
  modalShow,
  onHideFunction,
  roleList,
  users,
}) {
  const router = useRouter();
  const [state] = useState({
    error: false,
    message: "",
    completed: false,
  });
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Modify selected users role</h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-2 mb-2">
          <Form>
            <ErrorMessage
              message={state.message}
              messageStatus={state.error && state.completed}
            />
            <Form.Group className="mb-4">
              <Form.Select className="input-style shadow-sm" id="role">
                {Object.values(roleList).map((val) => (
                  <option key={val} value={val}>
                    {val}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>
            <Container>
              <Row>
                <Col>
                  <Button
                    variant="mainBlue"
                    className="w-100"
                    onClick={async (e) => {
                      e.preventDefault();
                      let role = document.getElementById("role").value;
                      let formData = new FormData();
                      formData.set("role", role);
                      let errorCount = 0;
                      for (let index = 0; index < users.length; index++) {
                        let result = await modifyUserRole(
                          users[index],
                          {},
                          formData,
                        );
                        if (result.error) errorCount++;
                      }
                      if (errorCount > 0) {
                        state.error = true;
                        state.message = `Error: ${errorCount} users has not been modified.`;
                        state.completed = true;
                      } else {
                        state.error = false;
                        state.message = "";
                        state.completed = true;
                        router.refresh();
                        onHideFunction();
                      }
                    }}
                  >
                    Save
                  </Button>
                </Col>
                <Col>
                  <Button
                    variant="red"
                    className="w-100"
                    onClick={() => {
                      state.error = false;
                      state.message = "";
                      state.completed = false;
                      router.refresh();
                      onHideFunction();
                    }}
                  >
                    Close
                  </Button>
                </Col>
              </Row>
            </Container>
          </Form>
        </Container>
      </Modal.Body>
    </Modal>
  );
}

ModifySelectedUserRole.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  roleList: PropTypes.object.isRequired,
  users: PropTypes.object.isRequired,
};

export default ModifySelectedUserRole;
