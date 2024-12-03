"use client";

import Image from "next/image";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Offcanvas, Container, Row, Col, Button, Stack } from "react-bootstrap";
import getRestClientInfo from "@/utils/clients/get_rest_info";
import dropdown_big_down from "../../../../public/icons/dropdown_big_down.png";

/**
 * Create offcanvas that allow to view more information about client.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {{clientName: string, user: string, nip: Number|undefined, street: string, city: string, postal: string}} props.client Chosen client to view.
 * @param {boolean} props.isOrg True if org view is activated.
 * @return {JSX.Element} Offcanvas element
 */
function ViewClientOffcanvas({ showOffcanvas, hideFunction, client, isOrg }) {
  // download holder
  const [restInfo, setRestInfo] = useState({
    creditLimit: null,
    availability: "",
    daysForRealization: null,
  });
  // download data
  useEffect(() => {
    if (showOffcanvas) {
      getRestClientInfo(client.clientId).then((data) => {
        if (data !== null) {
          setRestInfo(data);
        } else {
          restInfo.availability = "Error: could not download";
        }
      });
    }
  }, [showOffcanvas, client.clientId]);
  return (
    <Offcanvas
      className="h-100 minScalableWidth"
      show={showOffcanvas}
      onHide={hideFunction}
      placement="bottom"
    >
      <Offcanvas.Header className="border-bottom-grey px-xl-5">
        <Container className="px-3" fluid>
          <Row>
            <Col xs="10" className="d-flex align-items-center">
              <p className="blue-main-text h4 mb-0">{client.clientName}</p>
            </Col>
            <Col xs="2" className="ps-1 text-end">
              <Button
                variant="as-link"
                onClick={() => {
                  hideFunction();
                }}
                className="ps-2"
              >
                <Image src={dropdown_big_down} alt="Hide" />
              </Button>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Header>
      <Offcanvas.Body className="px-4 px-xl-5 mx-1 mx-xl-3 pb-0" as="div">
        <Container className="p-0" fluid>
          <Row>
            <Col>
              <Stack className="pt-3" gap={3}>
                {isOrg && client.users ? (
                  <Container className="px-1 ms-0">
                    <p className="mb-1 blue-main-text">Users:</p>
                    <ul className="mb-0">
                      {Object.values(client.users).map((user) => {
                        return <li key={user}>{user}</li>;
                      })}
                    </ul>
                  </Container>
                ) : null}
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Nip:</p>
                  <p className="mb-1">{client.nip ? client.nip : "-"}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Address:</p>
                  <p className="mb-1">{client.street + " " + client.city}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Postal code:</p>
                  <p className="mb-1">{client.postal}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Credit limit:</p>
                  <p className="mb-1">{restInfo.creditLimit}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Availability:</p>
                  <p className="mb-1">
                    {restInfo.availability +
                      " (" +
                      restInfo.daysForRealization +
                      ")"}
                  </p>
                </Container>
              </Stack>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

ViewClientOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  client: PropTypes.object.isRequired,
  isOrg: PropTypes.bool.isRequired,
};

export default ViewClientOffcanvas;
