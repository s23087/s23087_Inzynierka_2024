"use client";

import Image from "next/image";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Offcanvas, Container, Row, Col, Button, Stack } from "react-bootstrap";
import dropdown_big_down from "../../../../../public/icons/dropdown_big_down.png";
import download_off from "../../../../../public/icons/download_off.png";
import download_on from "../../../../../public/icons/download_on.png";
import getInvoiceFile from "@/utils/documents/download_invoice";
import getRestRequest from "@/utils/documents/get_rest_request";
function ViewRequestOffcanvas({ showOffcanvas, hideFunction, request, isOrg }) {
  const [note, setNote] = useState("");
  const [requestPath, setRequestPath] = useState("");
  useEffect(() => {
    if (showOffcanvas) {
      let restInfo = getRestRequest(request.id);
      restInfo.then((data) => {
        setNote(data.note);
        setRequestPath(data.path);
      });
    }
  }, [showOffcanvas]);
  // Download bool
  const [isDownloading, setIsDowlonding] = useState(false);
  // Styles
  const statusColorMap = {
    Fulfilled: "var(--main-green)",
    "In progress": "var(--main-yellow)",
    Rejected: "var(--sec-red)",
  };
  const statusStyle = {
    backgroundColor: statusColorMap[request.status],
    color:
      statusColorMap[request.status] === "var(--sec-red)"
        ? "var(--text-main-color)"
        : "var(--text-black-color)",
    justifyContent: "center",
  };
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
            <Col xs="7" lg="9" xl="10" className="d-flex align-items-center">
              <p className="blue-main-text h4 mb-0">Request view</p>
            </Col>
            <Col
              xs="3"
              lg="2"
              xl="1"
              className="d-flex align-items-center justify-content-end pe-1"
            >
              <Button
                variant="as-link"
                className="p-0"
                disabled={isDownloading || !requestPath}
                onClick={async () => {
                  setIsDowlonding(true);
                  let file = await getInvoiceFile(requestPath);
                  if (file) {
                    let parsed = JSON.parse(file);
                    let buffer = Buffer.from(parsed.data);
                    let blob = new Blob([buffer], { type: "application/pdf" });
                    var url = URL.createObjectURL(blob);
                    let downloadObject = document.createElement("a");
                    downloadObject.href = url;
                    downloadObject.download = requestPath.substring(
                      requestPath.lastIndexOf("/"),
                      requestPath.length,
                    );
                    downloadObject.click();
                  }
                  setIsDowlonding(false);
                }}
              >
                <Image
                  src={requestPath ? download_on : download_off}
                  alt="download button"
                />
              </Button>
            </Col>
            <Col xs="2" lg="1" className="ps-1 text-end">
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
            <Col xs="12" md="6">
              <Stack className="pt-3" gap={3}>
                <Container className="px-1 ms-0">
                  <Row>
                    <Col className="me-auto">
                      <p className="mb-1 blue-main-text">Recevier:</p>
                      <p className="mb-1">{request.username}</p>
                    </Col>
                  </Row>
                </Container>
                <Container className="px-1 ms-0">
                  <Row>
                    <Col className="me-auto">
                      <p className="mb-1 blue-main-text">Object type:</p>
                      <p className="mb-1">{request.objectType}</p>
                    </Col>
                  </Row>
                </Container>
                <Container className="px-1 ms-0">
                  <span
                    className="spanStyle rounded-span d-flex"
                    style={statusStyle}
                  >
                    <p className="mb-1">{request.status}</p>
                  </span>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Note:</p>
                  <p className="mb-1">{note ? note : "-"}</p>
                </Container>
              </Stack>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

ViewRequestOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  request: PropTypes.object.isRequired,
  isOrg: PropTypes.bool.isRequired,
};

export default ViewRequestOffcanvas;
