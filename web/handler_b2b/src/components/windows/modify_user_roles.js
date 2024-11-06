"use client";

import PropTypes from "prop-types";
import { useFormState } from "react-dom";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";
import modifyUserRole from "@/utils/roles/modify_user_role";
import { useRouter } from "next/navigation";
import ErrorMessage from "../smaller_components/error_message";

/**
 * Modal element that allow to change user role.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.modalShow Modal show parameter.
 * @param {Function} props.onHideFunction Function that will close modal (set modalShow to false).
 * @param {Array<string>} props.roleList Role list.
 * @param {{userId: Number, username: string, surname: string}} props.user Object that contain user info.
 * @return {JSX.Element} Modal element
 */
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
  // Styles
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
                      // reset state and close modal
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
