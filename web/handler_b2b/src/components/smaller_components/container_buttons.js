import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";

function ContainerButtons({ selected }) {
  const buttonStyle = {
    "min-width": "77px",
    "max-width": "95px",
  };
  return (
    <Container className="h-100" fluid>
      <Row className="align-items-center justify-content-center h-100">
        <Col className="pe-2" xs="3" sm="auto">
          <Button
            variant="mainBlue"
            className="rounded-span w-100 p-0"
            style={buttonStyle}
          >
            {selected ? "Deselect" : "Select"}
          </Button>
        </Col>
        <Col className="px-2" xs="3" sm="auto">
          <Button
            variant="red"
            className="rounded-span w-100"
            style={buttonStyle}
          >
            Delete
          </Button>
        </Col>
        <Col className="px-2" xs="3" sm="auto">
          <Button
            variant="mainBlue"
            className="rounded-span w-100"
            style={buttonStyle}
          >
            View
          </Button>
        </Col>
        <Col className="ps-2" xs="3" sm="auto">
          <Button
            variant="mainBlue"
            className="rounded-span w-100"
            style={buttonStyle}
          >
            Modify
          </Button>
        </Col>
      </Row>
    </Container>
  );
}

ContainerButtons.propTypes = {
  selected: PropTypes.bool.isRequired,
};

export default ContainerButtons;
