import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";

/**
 * Object container buttons.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.selected True if container is in selected mode.
 * @param {Function} [props.selectAction=] Action that trigger object selection.
 * @param {Function} [props.unselectAction=] Action that trigger object unselect.
 * @param {Function} [props.deleteAction=] Action that open object delete modal.
 * @param {Function} [props.viewAction=] Action that open object view offcanvas.
 * @param {Function} [props.modifyAction=] Action that open object modify offcanvas.
 * @param {Function} [props.completeAction=] Action that trigger request status change to fulfilled. Available only if is_request is true.
 * @param {Function} [props.rejectAction=] Action that trigger request status change to reject. Available only if is_request is true.
 * @param {boolean} [props.is_request=false] True if object is request.
 * @param {boolean} [props.rejectUnaval=false] True if reject button should be disabled.
 * @param {boolean} [props.completeUnaval=false] True if complete button should be disabled.
 * @return {JSX.Element} Container element
 */
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
  rejectUnaval = false,
  completeUnaval = false,
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
            disabled={completeUnaval && is_request}
          >
            {is_request ? "Complete" : "View"}
          </Button>
        </Col>
        <Col className="ps-2" xs="3" sm="auto">
          <Button
            variant={is_request ? "red" : "mainBlue"}
            className="basicButtonStyle rounded-span w-100"
            onClick={is_request ? rejectAction : modifyAction}
            disabled={rejectUnaval && is_request}
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
  is_request: PropTypes.bool,
  completeUnaval: PropTypes.bool,
  rejectUnaval: PropTypes.bool,
};

export default ContainerButtons;
