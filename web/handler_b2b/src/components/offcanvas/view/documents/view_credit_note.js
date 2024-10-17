"use client";

import Image from "next/image";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Offcanvas, Container, Row, Col, Button, Stack } from "react-bootstrap";
import getDocumentStatusStyle from "@/utils/documents/get_document_status_color";
import dropdown_big_down from "../../../../../public/icons/dropdown_big_down.png";
import download_off from "../../../../../public/icons/download_off.png";
import download_on from "../../../../../public/icons/download_on.png";
import getFileFormServer from "@/utils/documents/download_file";
import getRestCreditNote from "@/utils/documents/get_rest_credit_note";
import CreditNoteTable from "@/components/tables/credit_table";
function ViewCreditNoteOffcanvas({
  showOffcanvas,
  hideFunction,
  creditNote,
  isYourCredit,
}) {
  const [restInfo, setRestInfo] = useState({
    creditNoteNumber: "",
    currencyName: "",
    note: "",
    path: "",
    creditItems: [],
  });
  const [creditPath, setCreditPath] = useState("");
  useEffect(() => {
    if (showOffcanvas) {
      getRestCreditNote(creditNote.creditNoteId)
      .then((data) => {
        if (data !== null){
          if (data.length === 0){
            setRestInfo({
              creditNoteNumber: "not found",
              currencyName: "not found",
              note: "not found",
              path: "not found",
              creditItems: [],
            })
            return;
          }
          setRestInfo(data);
          setCreditPath(data.path);
        } else {
          setRestInfo({
            creditNoteNumber: "connection error",
            currencyName: "connection error",
            note: "connection error",
            path: "connection error",
            creditItems: [],
          })
        }
        })
    }
  }, [showOffcanvas]);
  // Download bool
  const [isDownloading, setIsDowlonding] = useState(false);
  // Styles
  let statusTextColor = getDocumentStatusStyle(
    creditNote.isPaid ? "Paid" : "Unpaid",
  ).backgroundColor;
  const paymentStatusStyle = {
    color:
      statusTextColor === "var(--main-yellow)"
        ? "var(--warning-color)"
        : statusTextColor,
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
              <p className="blue-main-text h4 mb-0">Credit note view</p>
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
                disabled={isDownloading}
                onClick={async () => {
                  setIsDowlonding(true);
                  let file = await getFileFormServer(creditPath);
                  if (file) {
                    let parsed = JSON.parse(file);
                    let buffer = Buffer.from(parsed.data);
                    let blob = new Blob([buffer], { type: "application/pdf" });
                    let url = URL.createObjectURL(blob);
                    let downloadObject = document.createElement("a");
                    downloadObject.href = url;
                    downloadObject.download = creditPath.substring(
                      creditPath.lastIndexOf("/"),
                      creditPath.length,
                    );
                    downloadObject.click();
                  }
                  setIsDowlonding(false);
                }}
              >
                <Image
                  src={creditPath === "" ? download_off : download_on}
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
                className="ps-2 pe-0"
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
                      <p className="mb-1 h5">{restInfo.creditNoteNumber}</p>
                    </Col>
                  </Row>
                </Container>
                {creditNote.user ? (
                  <Container className="px-1 ms-0">
                    <Row>
                      <Col className="me-auto">
                        <p className="mb-1 blue-main-text">Users:</p>
                        <p className="mb-1">{creditNote.user}</p>
                      </Col>
                    </Row>
                  </Container>
                ) : null}
                <Container className="px-1 ms-0">
                  <Row>
                    <Col className="me-auto">
                      <p className="mb-1 blue-main-text">For Invoice:</p>
                      <p className="mb-1">{creditNote.invoiceNumber}</p>
                    </Col>
                  </Row>
                </Container>
                <Container className="px-1 ms-0">
                  <Row>
                    <Col className="me-auto">
                      <p className="mb-1 blue-main-text">Date:</p>
                      <p className="mb-1">{creditNote.date.substring(0, 10)}</p>
                    </Col>
                  </Row>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">
                    {isYourCredit ? "Source:" : "For:"}
                  </p>
                  <p className="mb-1">{creditNote.clientName}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Status:</p>
                  <p className="mb-1" style={paymentStatusStyle}>
                    {creditNote.isPaid ? "Paid" : "Unpaid"}
                  </p>
                </Container>
                <Container className="px-1 ms-0">
                  <span
                    className="spanStyle rounded-span d-flex"
                    style={getDocumentStatusStyle(
                      creditNote.inSystem ? "In system" : "Not in system",
                    )}
                  >
                    <p className="mb-1">
                      {creditNote.inSystem ? "In system" : "Not in system"}
                    </p>
                  </span>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Note:</p>
                  <p className="mb-1">{restInfo.note ? restInfo.note : "-"}</p>
                </Container>
              </Stack>
            </Col>
            <Col xs="12" md="6" lg="4" className="px-0 offset-lg-2">
              <Container
                className="pt-5 pt-md-3 text-center overflow-x-scroll"
                fluid
              >
                {restInfo.creditItems.length > 0 ? (
                  <CreditNoteTable
                    creditItems={restInfo.creditItems}
                    currency={restInfo.currencyName}
                  />
                ) : null}
              </Container>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

ViewCreditNoteOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  creditNote: PropTypes.object.isRequired,
  isYourCredit: PropTypes.bool.isRequired,
};

export default ViewCreditNoteOffcanvas;
