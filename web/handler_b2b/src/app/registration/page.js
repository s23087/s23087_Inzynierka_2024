"use client";

import { useState } from "react";
import { Container, Row, Col, Button } from "react-bootstrap";
import Image from "next/image";
import Link from "next/link";
import UserDiffModal from "@/components/registration/user_diffrences_modal";
import BigLogo from "../../../public/big_logo.png";
import RegistrationChooser from "../../components/registration/registration_chooser";
import GrayQuestionmark from "../../../public/icons/gray_questionmark.png";

export default function Registration() {
  const [show, setShow] = useState(false);

  const openUserInfo = () => setShow(true);
  const closeUserInfo = () => setShow(false);

  return (
    <main className="d-flex h-100 w-100 justify-content-center">
      <Container className="mb-5 px-5 mx-4 logo-spacing-sm">
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
            <Link href="/">
              <Button variant="as-link">&lt;&lt; Back to login</Button>
            </Link>
          </Col>
        </Row>
        <Row className="mb-3">
          <Col>
            <Container className="d-flex mt-4 justify-content-center">
              <Image src={GrayQuestionmark} alt="Question icon" />
              <Button
                onClick={openUserInfo}
                variant="as-link"
                className="small-text pt-0 d-flex align-items-top"
              >
                <p className="grey-sec-text mb-0">
                  Click here to learn about differences
                </p>
              </Button>
            </Container>
          </Col>
        </Row>
      </Container>

      <UserDiffModal modalShow={show} onHideFunction={closeUserInfo} />
    </main>
  );
}
