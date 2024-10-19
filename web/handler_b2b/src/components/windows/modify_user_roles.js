"use client";

import PropTypes from "prop-types";
import { useFormState } from "react-dom";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import modifyUserRole from "@/utils/roles/modify_user_role";
import { useRouter } from "next/navigation";
import ErrorMessage from "../smaller_components/error_message";

function ModifyUserRole({ modalShow, onHideFunction, roleList, user }) {
  const router = useRouter();
  const [state, formAction] = useFormState(
    modifyUserRole.bind(null, user.userId),
    {
      error: false,
      message: "",
      completed: false,
    },
  );
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };
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
              <h5 className="mb-0 mt-3">
                Modify {user.username + " " + user.surname} role
              </h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-2 mb-2">
          <Form action={formAction}>
            <Row>
              <p
                className="text-start mb-1 px-3 green-main-text small-text"
                style={!state.error && state.completed ? unhidden : hidden}
              >
                {state.message}
              </p>
            </Row>
            <ErrorMessage
              message={state.message}
              messageStatus={state.error && state.completed}
            />
            <Form.Group className="mb-4">
              <Form.Select className="input-style shadow-sm" name="role">
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
                  <Button variant="mainBlue" className="w-100" type="submit">
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

ModifyUserRole.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  roleList: PropTypes.object.isRequired,
  user: PropTypes.object.isRequired,
};

export default ModifyUserRole;
