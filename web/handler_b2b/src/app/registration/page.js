"use client";

import { useState } from "react";
import { Modal, Container, Row, Col } from "react-bootstrap";
import Image from "next/image";
import CloseIcon from "../../../public/icons/close_black.png";
import MoreIcon from "../../../public/icons/more.png";
import BigLogo from "../../../public/big_logo.png";
import RegistrationChooser from "../../components/registration/registration_chooser";
import GrayQuestionmark from "../../../public/icons/gray_questionmark.png";

export default function Registration() {
  const [show, setShow] = useState(false);
  const modalContainerStyle = {
    height: "360px",
    "font-size": "14px",
  };
  const diffrentCursorStyle = {
    cursor: "pointer",
  };

  const openUserInfo = () => setShow(true);
  const closeUserInfo = () => setShow(false);

  return (
    <main className="d-flex h-100 w-100 justify-content-center align-items-center">
      <Container className="mb-5 px-5 mx-4">
        <Row className="mb-4">
          <Col className="text-center">
            <Image src={BigLogo} alt="Logo" />
          </Col>
        </Row>
        <Row className="mb-3">
          <Col>
            <RegistrationChooser />
          </Col>
        </Row>
        <Row className="text-center">
          <Col>
            <p>
              <a
                className="blue-main-text link-underline link-underline-opacity-0"
                href="/"
              >
                &lt;&lt; Back to login
              </a>
            </p>
          </Col>
        </Row>
        <Row className="mb-3">
          <Col>
            <Container className="d-flex mt-4 justify-content-center">
              <Image
                src={GrayQuestionmark}
                alt="Question icon"
                className="me-2 mt-1"
              />
              <p>
                <a
                  onClick={openUserInfo}
                  className="grey-sec-text small-text link-underline link-underline-opacity-0"
                  style={diffrentCursorStyle}
                >
                  Click here to learn about differences
                </a>
              </p>
            </Container>
          </Col>
        </Row>
      </Container>

      <Modal
        show={show}
        onHide={closeUserInfo}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered
        className="px-4"
      >
        <Modal.Body className="mx-1 pt-2">
          <Container className="mb-4">
            <Row>
              <Col className="mt-1">
                <h5 className="mb-0 mt-2">User types</h5>
              </Col>
              <Col className="d-flex justify-content-end pe-0">
                <Image
                  src={CloseIcon}
                  alt="Close icon"
                  onClick={closeUserInfo}
                  style={diffrentCursorStyle}
                />
              </Col>
            </Row>
          </Container>
          <Container
            className="overflow-y-scroll lh-sm"
            style={modalContainerStyle}
          >
            <h6 className="mb-1">Individual user:</h6>
            <p>
              Cras mattis consectetur purus sit amet fermentum. Cras justo odio,
              dapibus ac facilisis in, egestas eget quam. Morbi leo risus, porta
              ac consectetur ac, vestibulum at eros.
            </p>

            <p>
              Cras mattis consectetur purus sit amet fermentum. Cras justo odio,
              dapibus ac facilisis in, egestas eget quam. Morbi leo risus, porta
              ac consectetur ac, vestibulum at eros.
            </p>

            <h6 className="mb-1">Org user:</h6>
            <p>
              Cras mattis consectetur purus sit amet fermentum. Cras justo odio,
              dapibus ac facilisis in, egestas eget quam. Morbi leo risus, porta
              ac consectetur ac, vestibulum at eros.
            </p>

            <p>
              Cras mattis consectetur purus sit amet fermentum. Cras justo odio,
              dapibus ac facilisis in, egestas eget quam. Morbi leo risus, porta
              ac consectetur ac, vestibulum at eros.
            </p>
          </Container>
        </Modal.Body>
        <Modal.Footer className="justify-content-center py-1">
          <Image src={MoreIcon} alt="Three gray dot" />
        </Modal.Footer>
      </Modal>
    </main>
  );
}
