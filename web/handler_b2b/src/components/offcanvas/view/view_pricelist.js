"use client";

import Image from "next/image";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Offcanvas, Container, Row, Col, Button, Stack } from "react-bootstrap";
import dropdown_big_down from "../../../../public/icons/dropdown_big_down.png";
import getPricelistItems from "@/utils/pricelist/get_pricelist_items";
import PricelistTable from "@/components/tables/pricelist_table";
function ViewPricelistOffcanvas({ showOffcanvas, hideFunction, pricelist }) {
  const [pricelistItems, setRestInfo] = useState([]);
  useEffect(() => {
    if (showOffcanvas) {
      getPricelistItems(pricelist.pricelistId)
      .then((data) => {
        if (data === null) {
          setErrorDownload(true);
        } else {
          setRestInfo(data);
          setErrorDownload(false);
        }
      });
    }
  }, [showOffcanvas]);
  // Download bool
  const [errorDownload, setErrorDownload] = useState(false);
  // style
  const statusColor = {
    color:
      pricelist.status === "Active" ? "var(--main-green)" : "var(--sec-red)",
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
            <Col xs="8" className="d-flex align-items-center">
              <p className="blue-main-text h4 mb-0">Pricelist view</p>
            </Col>
            <Col xs="4" className="ps-1 text-end">
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
            <p className="h5 my-2">{pricelist.name}</p>
          </Row>
          <Row>
            <Col xs="12" md="6">
              <Stack className="pt-3" gap={3}>
                <Container className="px-1 ms-0">
                  <Row>
                    <Col xs="4" className="me-auto">
                      <p className="mb-1 blue-main-text">Created:</p>
                      <p className="mb-1">
                        {pricelist.created.substring(0, 10)}
                      </p>
                    </Col>
                    <Col xs="4">
                      <p className="mb-1 blue-main-text">Modified:</p>
                      <p className="mb-1">
                        {pricelist.modified.substring(0, 10)}
                      </p>
                    </Col>
                  </Row>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Product count:</p>
                  <p className="mb-1">{pricelist.totalItems}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Status:</p>
                  <p className="mb-1" style={statusColor}>
                    {pricelist.status}
                  </p>
                </Container>
              </Stack>
            </Col>
            <Col xs="12" md="6" lg="4" className="px-0 offset-lg-2">
              <Container
                className="pt-5 pt-md-0 text-center overflow-x-scroll"
                fluid
              >
                {pricelistItems.length > 0 ? (
                  <PricelistTable
                    items={pricelistItems}
                    currency={pricelist.currency}
                  />
                ) : null}
                {errorDownload ? "Could not download items." : null}
              </Container>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

ViewPricelistOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  pricelist: PropTypes.object.isRequired,
};

export default ViewPricelistOffcanvas;
