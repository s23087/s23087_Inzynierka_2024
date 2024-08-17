import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";

function ContainerButtons({ selected, is_request = false }) {
  const buttonStyle = {
    minWidth: "77px",
    maxWidth: "95px",
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
            variant={is_request ? "mainBlue" : "red"}
            className="rounded-span w-100"
            style={buttonStyle}
          >
            {is_request ? "View" : "Delete"}
          </Button>
        </Col>
        <Col className="px-2" xs="3" sm="auto">
          <Button
            variant={is_request ? "green" : "mainBlue"}
            className="rounded-span w-100 p-0"
            style={buttonStyle}
          >
            {is_request ? "Complete" : "View"}
          </Button>
        </Col>
        <Col className="ps-2" xs="3" sm="auto">
          <Button
            variant={is_request ? "red" : "mainBlue"}
            className="rounded-span w-100"
            style={buttonStyle}
          >
            {is_request ? "Reject" : "Modify"}
          </Button>
        </Col>
      </Row>
    </Container>
  );
}

ContainerButtons.propTypes = {
  selected: PropTypes.bool.isRequired,
  is_request: PropTypes.bool.isRequired,
};

export default ContainerButtons;
