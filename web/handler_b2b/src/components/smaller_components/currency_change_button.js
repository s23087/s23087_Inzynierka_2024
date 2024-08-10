import PropTypes from "prop-types";
import { Button, Container, Row, Col } from "react-bootstrap";

function CurrencyChangeButton({ currency }) {
  return (
    <Button variant="mainBlue" className="pt-0 pb-1">
      <Container>
        <Row>
          <Col>
            <p className="mb-0 small-text">Currency:</p>
          </Col>
        </Row>
        <Row>
          <Col>
            <p className="mb-0">{currency}</p>
          </Col>
        </Row>
      </Container>
    </Button>
  );
}

CurrencyChangeButton.PropTypes = {
  currency: PropTypes.string.isRequired,
};

export default CurrencyChangeButton;
