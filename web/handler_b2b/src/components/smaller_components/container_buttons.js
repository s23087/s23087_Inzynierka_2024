import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";

function ContainerButtons({
  selected,
  selectAction,
  unselectAction,
  deleteAction,
  viewAction,
  modifyAction,
  completeAction,
  rejectAction,
  is_request = false,
}) {
  return (
    <Container className="h-100" fluid>
      <Row className="align-items-center justify-content-center justify-content-xl-end h-100">
        <Col className="pe-2" xs="3" sm="auto">
          <Button
            variant="mainBlue"
            className="basicButtonStyle rounded-span w-100 p-0"
            onClick={selected ? unselectAction : selectAction}
          >
            {selected ? "Deselect" : "Select"}
          </Button>
        </Col>
        <Col className="px-2" xs="3" sm="auto">
          <Button
            variant={is_request ? "mainBlue" : "red"}
            className="basicButtonStyle rounded-span w-100"
            onClick={is_request ? viewAction : deleteAction}
          >
            {is_request ? "View" : "Delete"}
          </Button>
        </Col>
        <Col className="px-2" xs="3" sm="auto">
          <Button
            variant={is_request ? "green" : "mainBlue"}
            className="basicButtonStyle rounded-span w-100 p-0"
            onClick={is_request ? completeAction : viewAction}
          >
            {is_request ? "Complete" : "View"}
          </Button>
        </Col>
        <Col className="ps-2" xs="3" sm="auto">
          <Button
            variant={is_request ? "red" : "mainBlue"}
            className="basicButtonStyle rounded-span w-100"
            onClick={is_request ? rejectAction : modifyAction}
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
  selectAction: PropTypes.func,
  unselectAction: PropTypes.func,
  deleteAction: PropTypes.func,
  viewAction: PropTypes.func,
  modifyAction: PropTypes.func,
  completeAction: PropTypes.func,
  rejectAction: PropTypes.func,
  is_request: PropTypes.bool.isRequired,
};

export default ContainerButtons;
