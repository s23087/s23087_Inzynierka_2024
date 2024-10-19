"use client";

import { usePathname, useRouter, useSearchParams } from "next/navigation";
import PropTypes from "prop-types";
import { Modal, Container, Row, Col, Form, Button } from "react-bootstrap";

function PagationWindow({ windowShow, onHideFunction }) {
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  return (
    <Modal
      size="md"
      show={windowShow}
      centered
      className="px-4 minScalableWidth"
    >
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Pagation</h5>
            </Col>
          </Row>
        </Container>
        <Container className="mt-4 mb-2">
          <Form>
            <Form.Select className="main-grey-bg" id="select">
              <option value="5">5</option>
              <option value="10">10</option>
              <option value="20">20</option>
              <option value="30">30</option>
              <option value="60">60</option>
              <option value="80">80</option>
              <option value="100">100</option>
            </Form.Select>
            <Container className="pt-4">
              <Row>
                <Col>
                  <Button
                    variant="mainBlue"
                    className="w-100"
                    onClick={(e) => {
                      e.preventDefault();
                      let selectVal = document.getElementById("select").value;
                      const newParams = new URLSearchParams(params);
                      newParams.set("pagation", selectVal);
                      newParams.set("page", 1);
                      router.replace(`${pathName}?${newParams}`);
                      onHideFunction();
                    }}
                  >
                    Change
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
          </Form>
        </Container>
      </Modal.Body>
    </Modal>
  );
}

PagationWindow.propTypes = {
  windowShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
};

export default PagationWindow;
