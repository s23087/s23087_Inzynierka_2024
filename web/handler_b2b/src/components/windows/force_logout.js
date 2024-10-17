"use client";

import { useRouter } from "next/navigation";
import PropTypes from "prop-types";
import { Modal, Container, Row, Col, Button } from "react-bootstrap";
import logout from "@/utils/auth/logout";

function ForceLogutWindow({ modalShow }) {
  const router = useRouter();
  return (
    <Modal size="md" show={modalShow} centered className="px-4 minScalableWidth">
      <Modal.Body>
        <Container className="mt-3 mb-2">
          <Row>
            <p className="text-start mb-4 h3">Success!</p>
          </Row>
          <Row>
            <p className="text-start mb-4">
              Your password changed. Please log in again.
            </p>
          </Row>
          <Container>
            <Row>
              <Col>
                <Button
                  variant="mainBlue"
                  className="w-100"
                  onClick={async () => {
                    await logout();
                    router.push("/");
                  }}
                >
                  Logout
                </Button>
              </Col>
            </Row>
          </Container>
        </Container>
      </Modal.Body>
    </Modal>
  );
}

ForceLogutWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
};

export default ForceLogutWindow;
