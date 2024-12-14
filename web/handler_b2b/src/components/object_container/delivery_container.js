import PropTypes from "prop-types";
import { Container, Row, Col } from "react-bootstrap";
import ContainerButtons from "../smaller_components/container_buttons";
import getDeliveryStatusColor from "@/utils/deliveries/get_delivery_status_color";

/**
 * Create element that represent delivery object
 * @component
 * @param {Object} props
 * @param {{user: string, deliveryId: Number, status: string, waybill: Array<string>, deliveryCompany: string, estimated: string, proforma: string, clientName: string, delivered: string}} props.delivery Object value
 * @param {boolean} props.is_org True if org view is enabled
 * @param {boolean} props.selected True if container should show as selected
 * @param {boolean} props.isDeliveryToUser True if current type is "Deliveries to user"
 * @param {Function} props.selectAction Action that will activated after clicking select button
 * @param {Function} props.unselectAction Action that will activated after clicking unselect button
 * @param {Function} props.deleteAction Action that will activated after clicking delete button
 * @param {Function} props.viewAction Action that will activated after clicking view button
 * @param {Function} props.modifyAction Action that will activated after clicking modify button
 * @return {JSX.Element} Container element
 */
function DeliveryContainer({
  delivery,
  is_org,
  selected,
  isDeliveryToUser,
  selectAction,
  unselectAction,
  deleteAction,
  viewAction,
  modifyAction,
}) {
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  return (
    <Container
      className="py-3 black-text medium-text border-bottom-grey "
      style={selected ? containerBg : null}
      fluid
    >
      <Row className="mx-0 mx-md-3 mx-xl-3">
        <Col xs="12" md="6" lg="6" xl="4">
          {is_org ? (
            <Row className="mb-2">
              <Col className="d-flex">
              <span className="me-2 mt-1 userIconStyle" title="user icon"/>
                <span className="spanStyle main-blue-bg main-text d-flex rounded-span px-2 w-100 my-1">
                  <p className="mb-0">{delivery.user}</p>
                </span>
              </Col>
            </Row>
          ) : null}
          <Row className="gy-2">
            <Col className="pe-1" xs="auto">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">Id: {delivery.deliveryId}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span
                className="spanStyle d-flex rounded-span px-2"
                style={getDeliveryStatusColor(delivery.status)}
              >
                <p className="mb-0">
                  Status:{" "}
                  {delivery.status === "Delivered with issues"
                    ? "Has issues"
                    : delivery.status}
                </p>
              </span>
            </Col>
            <Col xs="12" className="mb-0">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 d-block text-truncate">
                <p className="mb-0">Waybill: {delivery.waybill.join(", ")}</p>
              </span>
            </Col>
            <Col xs="12">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">
                  Estimated delivery: {delivery.estimated.substring(0, 10)}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col xs="12" md="6" lg="6" xl="4" className="ps-xxl-5">
          <Row className="mt-1 mt-md-0 h-100 align-items-center">
            <Row className="pe-0">
              <Col className="pe-1" xs="auto">
                <span className="spanStyle main-grey-bg d-flex rounded-span px-2 d-block text-truncate">
                  <p className="mb-0">
                    {isDeliveryToUser ? "Source:" : "For:"}{" "}
                    {delivery.clientName}
                  </p>
                </span>
              </Col>
              <Col className="ps-1 pe-0">
                <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                  <p className="mb-0">
                    Delivered:{" "}
                    {delivery.delivered
                      ? delivery.delivered.substring(0, 10)
                      : "-"}
                  </p>
                </span>
              </Col>
              <Col xs="12" className="mb-0 mt-2 pe-0">
                <span className="spanStyle main-blue-bg main-text d-flex rounded-span px-2 d-block text-truncate">
                  <p className="mb-0">Proforma: {delivery.proforma}</p>
                </span>
              </Col>
              <Col xs="12" className="mb-1 mt-2 mb-sm-0 pe-0">
                <span className="spanStyle main-grey-bg d-flex rounded-span px-2 d-block text-truncate">
                  <p className="mb-0">
                    Delivery company: {delivery.deliveryCompany}
                  </p>
                </span>
              </Col>
            </Row>
          </Row>
        </Col>
        <Col xs="12" xl="4" className="px-0 pt-3 pt-xl-2 pb-2">
          <ContainerButtons
            selected={selected}
            selectAction={selectAction}
            unselectAction={unselectAction}
            deleteAction={deleteAction}
            viewAction={viewAction}
            modifyAction={modifyAction}
          />
        </Col>
      </Row>
    </Container>
  );
}

DeliveryContainer.propTypes = {
  delivery: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
  isDeliveryToUser: PropTypes.bool.isRequired, // true for user, false for client
  selectAction: PropTypes.func,
  unselectAction: PropTypes.func,
  deleteAction: PropTypes.func,
  viewAction: PropTypes.func,
  modifyAction: PropTypes.func,
};

export default DeliveryContainer;
