"use client";

import Image from "next/image";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Offcanvas, Container, Row, Col, Button, Stack } from "react-bootstrap";
import getDocumentStatusStyle from "@/utils/documents/get_document_status_color";
import dropdown_big_down from "../../../../public/icons/dropdown_big_down.png";
import download_off from "../../../../public/icons/download_off.png";
import download_on from "../../../../public/icons/download_on.png";
import getFileFormServer from "@/utils/documents/download_file";
import CreditNoteTable from "@/components/tables/credit_table";
import getRestProforma from "@/utils/proformas/get_rest_proforma";
function ViewProformaOffcanvas({
  showOffcanvas,
  hideFunction,
  proforma,
  isYourProforma,
  isOrg,
}) {
  const [restInfo, setRestInfo] = useState({
    taxes: 0,
    currencyValue: 0.0,
    currencyDate: "",
    paymentMethod: "Is loading",
    inSystem: false,
    note: "Is loading",
    items: [],
  });
  const [proformaPath, setProformaPath] = useState("");
  useEffect(() => {
    if (showOffcanvas) {
      let restProforma = getRestProforma(isYourProforma, proforma.proformaId);
      restProforma.then((data) => {
        if (data.length !== 0) {
          setRestInfo(data);
          setProformaPath(data.path);
        } else {
          setRestInfo({
            taxes: 0,
            currencyValue: 0.0,
            currencyDate: "Not found",
            paymentMethod: "Not found",
            inSystem: false,
            note: "Not found",
            items: [],
          });
        }
      });
    }
  }, [showOffcanvas]);
  // Download bool
  const [isDownloading, setIsDowlonding] = useState(false);
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
              <p className="blue-main-text h4 mb-0">Proforma view</p>
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
                  let file = await getFileFormServer(proformaPath);
                  if (file) {
                    let parsed = JSON.parse(file);
                    let buffer = Buffer.from(parsed.data);
                    let blob = new Blob([buffer], { type: "application/pdf" });
                    var url = URL.createObjectURL(blob);
                    let downloadObject = document.createElement("a");
                    downloadObject.href = url;
                    downloadObject.download = proformaPath.substring(
                      proformaPath.lastIndexOf("/"),
                      proformaPath.length,
                    );
                    downloadObject.click();
                  }
                  setIsDowlonding(false);
                }}
              >
                <Image
                  src={proformaPath === "" ? download_off : download_on}
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
          <Row className="ps-1">
            <p className="h5 my-2">{proforma.proformaNumber}</p>
          </Row>
          <Row>
            <Col xs="12" md="6">
              <Stack className="pt-3" gap={3}>
                {isOrg ? (
                  <Container className="px-1 ms-0">
                    <p className="mb-1 blue-main-text">User:</p>
                    <p className="mb-1">{proforma.user}</p>
                  </Container>
                ) : null}
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Date:</p>
                  <p className="mb-1">{proforma.date.substring(0, 10)}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">
                    {isYourProforma ? "Source:" : "For:"}
                  </p>
                  <p className="mb-1">{proforma.clientName}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Taxes:</p>
                  <p className="mb-1">{restInfo.taxes}%</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Currency:</p>
                  <p className="mb-1">
                    {proforma.currencyName === "PLN"
                      ? "PLN"
                      : restInfo.currencyValue +
                        " " +
                        proforma.currencyName +
                        " " +
                        restInfo.currencyDate.substring(0, 10)}
                  </p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Transport Cost:</p>
                  <p className="mb-1">
                    {proforma.transport + " " + proforma.currencyName}
                  </p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Payment method:</p>
                  <p className="mb-1">{restInfo.paymentMethod}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <span
                    className="spanStyle rounded-span d-flex"
                    style={getDocumentStatusStyle(
                      restInfo.inSystem ? "In system" : "Not in system",
                    )}
                  >
                    <p className="mb-1">
                      {restInfo.inSystem ? "In system" : "Not in system"}
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
                {restInfo.items.length > 0 ? (
                  <CreditNoteTable
                    creditItems={restInfo.items}
                    currency={proforma.currencyName}
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

ViewProformaOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  proforma: PropTypes.object.isRequired,
  isYourProforma: PropTypes.bool.isRequired,
  isOrg: PropTypes.bool.isRequired,
};

export default ViewProformaOffcanvas;
