import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col } from "react-bootstrap";
import ContainerButtons from "../smaller_components/container_buttons";
import user_small_icon from "../../../public/icons/user_small_icon.png";

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
  const statusColorMap = {
    Fullfiled: "var(--main-green)",
    "In transport": "var(--main-yellow)",
    "Delivered with issues": "var(--sec-red)",
    Preparing: "var(--main-yellow)",
    Rejected: "var(--sec-red)",
  };
  const statusBgStyle = {
    backgroundColor: statusColorMap[delivery.status],
    color:
      statusColorMap[delivery.status] === "var(--sec-red)"
        ? "var(--text-main-color)"
        : "var(--text-black-color)",
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
                <Image
                  src={user_small_icon}
                  alt="user small icon"
                  className="me-2 mt-1"
                />
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
                style={statusBgStyle}
              >
                <p className="mb-0">Status: {delivery.status}</p>
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
          />
        </Col>
      </Row>
    </Container>
  );
}

DeliveryContainer.PropTypes = {
  delivery: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
  isDeliveryToUser: PropTypes.bool.isRequired, // 1 for user, 0 for client
  selectAction: PropTypes.func,
  unselectAction: PropTypes.func,
  deleteAction: PropTypes.func,
  viewAction: PropTypes.func,
  modifyAction: PropTypes.func,
};

export default DeliveryContainer;
