import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";
import Image from "next/image";
import currency_arrow from "../../../public/icons/currency_arrow.png";
import currency_button from "../../../public/icons/currency_button.png";

/**
 * Container for currency trade values.
 * @component
 * @param {object} props Component props
 * @param {string} props.currency_name The currency name that exchange rate will be show.
 * @param {string} props.current_currency Current chosen by user currency.
 * @param {Number} props.exchange_rate Currency exchange rate.
 * @param {Function} props.buttonAction Action that will happen after clicking round button.
 * @param {boolean} props.isEven If true background will change from light blue to dark blue.
 * @return {JSX.Element} Container element
 */
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
        <Col xs="auto" className="me-auto">
          <span className="d-flex" style={spanStyle}>
            <p className="mb-0">1 {currency_name}</p>
            <Image className="mx-3" src={currency_arrow} alt="arrow" />
            <p className="mb-0">
              {Math.round(exchange_rate * 100) / 100} {current_currency}
            </p>
          </span>
        </Col>
        <Col xs="1" className="px-0 text-end">
          <Button
            className="px-0 pe-md-1"
            variant="as-link"
            onClick={buttonAction}
          >
            <Image src={currency_button} alt="button" />
          </Button>
        </Col>
      </Row>
    </Container>
  );
}

CurrencyHolder.propTypes = {
  currency_name: PropTypes.string.isRequired,
  current_currency: PropTypes.string.isRequired,
  exchange_rate: PropTypes.number.isRequired,
  buttonAction: PropTypes.func.isRequired,
  isEven: PropTypes.bool.isRequired,
};

export default CurrencyHolder;
