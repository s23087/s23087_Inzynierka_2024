import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";
import Image from "next/image";
import currency_arrow from "../../../public/icons/currency_arrow.png";
import currency_button from "../../../public/icons/currency_button.png";

function CurrencyHolder({
  currency_name,
  current_currency,
  exchange_rate,
  buttonAction,
  isEven,
}) {
  const containerBg = {
    backgroundColor: isEven ? "var(--sec-blue)" : "var(--main-blue)",
    height: "62px",
  };
  const spanStyle = {
    backgroundColor: isEven ? "var(--main-blue)" : "var(--sec-blue)",
    minWidth: "214px",
    maxWidth: "718px",
    borderRadius: "10px",
    height: "34px",
    alignItems: "center",
    justifyContent: "center",
  };
  return (
    <Container className="p-0 main-text big-text" style={containerBg} fluid>
      <Row className="align-items-center px-4 px-xl-5 mx-1 mx-xl-3 h-100">
        <Col xs="auto" className="ps-1">
          <p className="mb-0">{currency_name}</p>
        </Col>
        <Col xs="auto">
          <span className="d-flex" style={spanStyle}>
            <p className="mb-0">1 {currency_name}</p>
            <Image className="mx-3" src={currency_arrow} alt="arrow" />
            <p className="mb-0">
              {Math.round(exchange_rate * 100) / 100} {current_currency}
            </p>
          </span>
        </Col>
        <Col className="px-0 text-end">
          <Button className="px-0" variant="as-link" onClick={buttonAction}>
            <Image src={currency_button} alt="button" />
          </Button>
        </Col>
      </Row>
    </Container>
  );
}

CurrencyHolder.PropTypes = {
  currency_name: PropTypes.string.isRequired,
  current_currency: PropTypes.string.isRequired,
  exchange_rate: PropTypes.number.isRequired,
  buttonAction: PropTypes.func.isRequired,
  isEven: PropTypes.bool.isRequired,
};

export default CurrencyHolder;
