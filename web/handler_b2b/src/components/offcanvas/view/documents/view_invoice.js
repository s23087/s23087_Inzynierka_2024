"use client";

import Image from "next/image";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Offcanvas, Container, Row, Col, Button, Stack } from "react-bootstrap";
import getDocumentStatusStyle from "@/utils/documents/get_document_status_color";
import dropdown_big_down from "../../../../../public/icons/dropdown_big_down.png";
import download_off from "../../../../../public/icons/download_off.png";
import download_on from "../../../../../public/icons/download_on.png";
import getRestInvoice from "@/utils/documents/get_rest_invoice";
import getInvoiceFile from "@/utils/documents/download_invoice";
import InvoiceTable from "@/components/tables/invoice_table";
function ViewInvoiceOffcanvas({
  showOffcanvas,
  hideFunction,
  invoice,
  isYourInvoice,
}) {
  const [restInfo, setRestInfo] = useState({
    tax: 0,
    currencyValue: 0,
    currencyName: "",
    currencyDate: "",
    transportCost: 0,
    paymentType: "",
    note: "",
    creditNotes: [],
    items: [],
  });
  const [invoicePath, setInvoicePath] = useState("");
  useEffect(() => {
    if (showOffcanvas) {
      let restInfo = getRestInvoice(invoice.invoiceId, isYourInvoice);
      restInfo
        .then((data) => {
          setRestInfo(data);
          setInvoicePath(data.path);
        })
        .catch(() => {
          setRestInfo({
            tax: 0,
            currencyValue: 0,
            currencyName: "error",
            currencyDate: "error",
            transportCost: 0,
            paymentType: "error",
            note: "error",
            items: [],
          });
        });
    }
  }, [showOffcanvas, isYourInvoice]);
  // Download bool
  const [isDownloading, setIsDowlonding] = useState(false);
  // Styles
  let statusTextColor = getDocumentStatusStyle(
    invoice.paymentStatus,
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
            <Col xs="6" lg="9" xl="10" className="d-flex align-items-center">
              <p className="blue-main-text h4 mb-0">Invoice view</p>
            </Col>
            <Col
              xs="4"
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
                  let file = await getInvoiceFile(invoicePath);
                  if (file) {
                    let parsed = JSON.parse(file);
                    let buffer = Buffer.from(parsed.data);
                    let blob = new Blob([buffer], { type: "application/pdf" });
                    var url = URL.createObjectURL(blob);
                    let downloadObject = document.createElement("a");
                    downloadObject.href = url;
                    downloadObject.download = invoicePath.substring(
                      invoicePath.lastIndexOf("/"),
                      invoicePath.length,
                    );
                    downloadObject.click();
                  }
                  setIsDowlonding(false);
                }}
              >
                <Image
                  src={invoicePath === "" ? download_off : download_on}
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
            <p className="h5 my-2">{invoice.invoiceNumber}</p>
          </Row>
          <Row>
            <Col xs="12" md="6">
              <Stack className="pt-3" gap={3}>
                <Container className="px-1 ms-0">
                  <Row>
                    <Col xs="4" className="me-auto">
                      <p className="mb-1 blue-main-text">Date:</p>
                      <p className="mb-1">
                        {invoice.invoiceDate.substring(0, 10)}
                      </p>
                    </Col>
                    <Col xs="4">
                      <p className="mb-1 blue-main-text">Due date:</p>
                      <p className="mb-1">{invoice.dueDate.substring(0, 10)}</p>
                    </Col>
                  </Row>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">
                    {isYourInvoice ? "Source:" : "Buyer:"}
                  </p>
                  <p className="mb-1">{invoice.clientName}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Taxes:</p>
                  <p className="mb-1">{restInfo.tax}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Currency:</p>
                  <p className="mb-1">
                    {restInfo.currencyValue +
                      " " +
                      restInfo.currencyName +
                      " " +
                      restInfo.currencyDate.substring(0, 10)}
                  </p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Transport Cost:</p>
                  <p className="mb-1">
                    {restInfo.transportCost + " " + restInfo.currencyName}
                  </p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Payment method:</p>
                  <p className="mb-1">{restInfo.paymentType}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Status:</p>
                  <p className="mb-1" style={paymentStatusStyle}>
                    {invoice.paymentStatus}
                  </p>
                </Container>
                <Container className="px-1 ms-0">
                  <span
                    className="spanStyle rounded-span d-flex"
                    style={getDocumentStatusStyle(
                      invoice.inSystem ? "In system" : "Not in system",
                    )}
                  >
                    <p className="mb-1">
                      {invoice.inSystem ? "In system" : "Not in system"}
                    </p>
                  </span>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Note:</p>
                  <p className="mb-1">{restInfo.note ? restInfo.note : "-"}</p>
                </Container>
                {restInfo.creditNotes.length > 0 ? (
                  <Container className="px-1 ms-0">
                    <p className="mb-1 blue-main-text">Credit notes:</p>
                    <ul className="mb-0">
                      {restInfo.creditNotes.map((val) => {
                        return <li key={val}>{val}</li>;
                      })}
                    </ul>
                  </Container>
                ) : null}
              </Stack>
            </Col>
            <Col xs="12" md="6" lg="4" className="px-0 offset-lg-2">
              <Container className="pt-5 text-center overflow-x-scroll">
                {restInfo.items.length > 0 ? (
                  <InvoiceTable
                    items={restInfo.items}
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

ViewInvoiceOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  invoice: PropTypes.object.isRequired,
  isYourInvoice: PropTypes.bool.isRequired,
};

export default ViewInvoiceOffcanvas;
