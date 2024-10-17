"use client";

import Image from "next/image";
import PropTypes from "prop-types";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import { Offcanvas, Container, Row, Col, Button } from "react-bootstrap";
import getCurrencyValues from "@/utils/flexible/get_currency_values";
import dropdown_big_down from "../../../public/icons/dropdown_big_down.png";
import CurrencyHolder from "../smaller_components/currency_holder";
function CurrencyOffcanvas({ showOffcanvas, hideFunction, current_currency }) {
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  const newParams = new URLSearchParams(params);
  const changeCurrency = (newCurr) => {
    newParams.set("currency", newCurr);
    router.replace(`${pathName}?${newParams}`);
  };
  const [eurData, setEurData] = useState({
    rates: [
      {
        effectiveDate: "",
        mid: 0,
      },
    ],
  });
  const [usdData, setUsdData] = useState({
    rates: [
      {
        effectiveDate: "",
        mid: 0,
      },
    ],
  });
  useEffect(() => {
    if (showOffcanvas) {
      getCurrencyValues("EUR")
      .then((data) => {
        if (data !== null){
          setEurData(data)
        } else {
          setEurData({
            rates: [
              {
                effectiveDate: "Connection error",
                mid: 0,
              },
            ],
          })
        }
      });
      getCurrencyValues("USD")
      .then((data) => {
        if (data !== null){
          setUsdData(data)
        } else {
          setUsdData({
            rates: [
              {
                effectiveDate: "Connection error",
                mid: 0,
              },
            ],
          })
        }
      });
    }
  }, [showOffcanvas]);
  const spanStyle = {
    width: "80px",
    height: "40px",
    borderRadius: "10px",
    alignItems: "center",
    justifyContent: "center",
  };
  return (
    <Offcanvas
      className="h-100 minScalableWidth"
      show={showOffcanvas}
      onHide={hideFunction}
      placement="bottom"
    >
      <Offcanvas.Header className="border-bottom-grey px-4 px-xl-5 mx-1 mx-xl-3">
        <Container className="px-3" fluid>
          <Row className="align-items-center">
            <Col
              xs="7"
              lg="9"
              xl="10"
              className="d-flex align-items-center p-0"
            >
              <p className="blue-main-text h4 mb-0">Change currency</p>
            </Col>
            <Col xs="4" lg="2" xl="1" className="p-0">
              <span
                className="main-text main-blue-bg d-flex mx-auto me-md-0"
                style={spanStyle}
              >
                {current_currency}
              </span>
            </Col>
            <Col xs="1" className="p-0 text-end">
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
      <Offcanvas.Body className="p-0" as="div">
        {current_currency === "PLN" ? (
          <>
            <CurrencyHolder
              currency_name="EUR"
              current_currency={current_currency}
              exchange_rate={eurData.rates[0].mid}
              buttonAction={() => {
                changeCurrency("EUR");
              }}
              isEven={false}
            />
            <CurrencyHolder
              currency_name="USD"
              current_currency={current_currency}
              exchange_rate={usdData.rates[0].mid}
              buttonAction={() => {
                changeCurrency("USD");
              }}
              isEven={true}
            />
          </>
        ) : null}
        {current_currency === "EUR" ? (
          <>
            <CurrencyHolder
              currency_name="PLN"
              current_currency={current_currency}
              exchange_rate={1 / eurData.rates[0].mid}
              buttonAction={() => {
                changeCurrency("PLN");
              }}
              isEven={false}
            />
            <CurrencyHolder
              currency_name="USD"
              current_currency={current_currency}
              exchange_rate={usdData.rates[0].mid}
              buttonAction={() => {
                changeCurrency("USD");
              }}
              isEven={true}
            />
          </>
        ) : null}
        {current_currency === "USD" ? (
          <>
            <CurrencyHolder
              currency_name="EUR"
              current_currency={current_currency}
              exchange_rate={eurData.rates[0].mid}
              buttonAction={() => {
                changeCurrency("EUR");
              }}
              isEven={false}
            />
            <CurrencyHolder
              currency_name="PLN"
              current_currency={current_currency}
              exchange_rate={1 / usdData.rates[0].mid}
              buttonAction={() => {
                changeCurrency("PLN");
              }}
              isEven={true}
            />
          </>
        ) : null}
        <Container
          className="main-grey-bg py-2 fixed-bottom w-100 border-top-grey px-4 px-xl-5"
          fluid
        >
          <Row className="mx-auto minScalableWidth grey-sec-text">
            <Col>
              <p className="mb-0">Updated from NBP:</p>
              <ul className="mb-0">
                <li>EUR - {eurData.rates[0].effectiveDate}</li>
                <li>USD - {usdData.rates[0].effectiveDate}</li>
              </ul>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

CurrencyOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  current_currency: PropTypes.string.isRequired
};

export default CurrencyOffcanvas;
