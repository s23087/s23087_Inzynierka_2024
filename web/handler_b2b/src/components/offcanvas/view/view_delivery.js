"use client";

import Image from "next/image";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Offcanvas, Container, Row, Col, Button, Stack } from "react-bootstrap";
import dropdown_big_down from "../../../../public/icons/dropdown_big_down.png";
import getDeliveryStatusColor from "@/utils/deliveries/get_delivery_status_color";
import DeliveryTable from "@/components/tables/delivery_table";
import getRestDelivery from "@/utils/deliveries/get_rest_delivery";
import AddNoteWindow from "@/components/windows/addNote";
import ErrorMessage from "@/components/smaller_components/error_message";
import setDeliveryStatus from "@/utils/deliveries/set_delivery_status";
import { useRouter } from "next/navigation";
function ViewDeliveryOffcanvas({
  showOffcanvas,
  hideFunction,
  delivery,
  isOrg,
  isDeliveryToUser,
}) {
  const router = useRouter();
  const [restInfo, setRestInfo] = useState({
    notes: [],
    items: [],
  });
  const [showAddNote, setShowAddNote] = useState(false);
  const [downloadError, setDownloadError] = useState(false);
  const [hasChanged, setHasChanged] = useState(false); // switch to other value when note has been added
  let statusColor = getDeliveryStatusColor(delivery.status).backgroundColor;
  useEffect(() => {
    if (showOffcanvas) {
      let restInfo = getRestDelivery(delivery.deliveryId);
      restInfo
        .then((data) => setRestInfo(data))
        .catch(() => setDownloadError(true));
    }
  }, [showOffcanvas, hasChanged]);
  // change status
  const [isLoading, setIsLoading] = useState(false);
  const [changeStatusError, setChangeStatusError] = useState(false);
  // styles
  const marginBottom = {
    marginBottom: "100px",
  };
  return (
    <Offcanvas
      className="h-100 minScalableWidth"
      show={showOffcanvas}
      onHide={hideFunction}
      placement="bottom"
    >
      <Offcanvas.Header className="border-bottom-grey px-xl-5">
        <Container className="px-3" fluid>
          <Row>
            <Col xs="10" className="d-flex align-items-center">
              <p className="blue-main-text h4 mb-0">
                Delivery id: {delivery.deliveryId}
              </p>
            </Col>
            <Col xs="2" className="ps-1 text-end">
              <Button
                variant="as-link"
                onClick={() => {
                  hideFunction();
                }}
                className="ps-2"
              >
                <Image src={dropdown_big_down} alt="Hide" />
              </Button>
            </Col>
          </Row>
          <Row>
            <ErrorMessage
              message="Delivery does not exist."
              messageStatus={changeStatusError}
            />
          </Row>
        </Container>
      </Offcanvas.Header>
      <Offcanvas.Body className="px-4 px-xl-5 mx-1 mx-xl-3 pb-0" as="div">
        <Container className="p-0" style={marginBottom} fluid>
          <Row>
            <Col>
              <Stack className="pt-3" gap={3}>
                <ErrorMessage
                  message="Could not download items and notes."
                  messageStatus={downloadError}
                />
                {isOrg ? (
                  <Container className="px-1 ms-0">
                    <p className="mb-1 blue-main-text">User:</p>
                    <p className="mb-1">{delivery.user}</p>
                  </Container>
                ) : null}
                <Container className="px-1 ms-0">
                  <Row>
                    <Col xs="7" className="me-auto">
                      <p className="mb-1 blue-main-text">Estimated delivery:</p>
                      <p className="mb-1">
                        {delivery.estimated.substring(0, 10)}
                      </p>
                    </Col>
                    <Col xs="5">
                      <p className="mb-1 blue-main-text">Delivery date:</p>
                      <p className="mb-1">
                        {delivery.delivered
                          ? delivery.delivered.substring(0, 10)
                          : "-"}
                      </p>
                    </Col>
                  </Row>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Proforma:</p>
                  <p className="mb-1">{delivery.proforma}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Delivery company:</p>
                  <p className="mb-1">{delivery.deliveryCompany}</p>
                </Container>
                <Container className="px-1 ms-0">
                  <p className="mb-1 blue-main-text">Waybill:</p>
                  <ul className="mb-1">
                    {Object.values(delivery.waybill).map((val) => {
                      return <li key={val}>{val}</li>;
                    })}
                  </ul>
                </Container>
                <Container className="px-1 ms-0" key={delivery.status}>
                  <p className="mb-1 blue-main-text">Status:</p>
                  <p
                    className="mb-1"
                    style={{
                      color:
                        statusColor === "var(--main-yellow)"
                          ? "var(--warning-color)"
                          : statusColor,
                    }}
                  >
                    {delivery.status}
                  </p>
                </Container>
                <Container className="px-1 ms-0" key={restInfo.notes}>
                  <p className="mb-1 blue-main-text">Notes:</p>
                  <p
                    className="mb-1 noteContainer break-spaces overflow-y-scroll p-2"
                    style={{ maxHeight: "150px" }}
                  >
                    {restInfo.notes.length === 0
                      ? "-"
                      : restInfo.notes.reduce(
                          (stacker, obj) =>
                            stacker +
                            obj.noteDate.replace("T", " ").substring(0, 19) +
                            " " +
                            obj.username +
                            "\n" +
                            obj.note +
                            "\n",
                          "",
                        )}
                  </p>
                  <Button
                    variant="secBlue"
                    className="mt-3 px-5"
                    onClick={() => setShowAddNote(true)}
                    disabled={downloadError}
                  >
                    Add note
                  </Button>
                  <AddNoteWindow
                    modalShow={showAddNote}
                    onHideFunction={() => setShowAddNote(false)}
                    deliveryId={delivery.deliveryId}
                    successFun={() => setHasChanged(!hasChanged)}
                  />
                </Container>
                <Container className="pt-3 text-center overflow-x-scroll px-0">
                  {restInfo.items.length > 0 ? (
                    <DeliveryTable items={restInfo.items} />
                  ) : null}
                </Container>
              </Stack>
            </Col>
          </Row>
        </Container>
        <Container className="main-grey-bg p-3 fixed-bottom w-100">
          <Row className="mx-auto minScalableWidth">
            {delivery.status === "In transport" ||
            delivery.status === "Preparing" ? (
              <>
                <Col>
                  <Button
                    variant="green"
                    className="w-100"
                    disabled={isLoading || changeStatusError}
                    onClick={async () => {
                      setIsLoading(true);
                      let result = await setDeliveryStatus(
                        delivery.deliveryId,
                        "Fulfilled",
                      );
                      if (result) {
                        delivery.status = "Fulfilled";
                        setIsLoading(false);
                        router.refresh();
                      } else {
                        setChangeStatusError(true);
                      }
                      setIsLoading(false);
                    }}
                  >
                    Complete
                  </Button>
                </Col>
                <Col>
                  <Button
                    variant="red"
                    className="w-100"
                    disabled={isLoading || changeStatusError}
                    onClick={async () => {
                      setIsLoading(true);
                      let statusName = isDeliveryToUser
                        ? "Delivered with issues"
                        : "Rejected";
                      let result = await setDeliveryStatus(
                        delivery.deliveryId,
                        statusName,
                      );
                      if (result) {
                        delivery.status = statusName;
                        setIsLoading(false);
                        router.refresh();
                      } else {
                        setChangeStatusError(true);
                      }
                      setIsLoading(false);
                    }}
                  >
                    {isDeliveryToUser ? "Has issues" : "Reject"}
                  </Button>
                </Col>
              </>
            ) : (
              <Col>
                <Button
                  variant="secBlue"
                  className="w-100"
                  disabled={isLoading || changeStatusError}
                  onClick={async () => {
                    setIsLoading(true);
                    let statusName = isDeliveryToUser
                      ? "In transport"
                      : "Preparing";
                    let result = await setDeliveryStatus(
                      delivery.deliveryId,
                      statusName,
                    );
                    if (result) {
                      delivery.status = statusName;
                      setIsLoading(false);
                      router.refresh();
                    } else {
                      setChangeStatusError(true);
                    }
                    setIsLoading(false);
                  }}
                >
                  Revert to {isDeliveryToUser ? "in transport" : "preparing"}
                </Button>
              </Col>
            )}
          </Row>
        </Container>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

ViewDeliveryOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  delivery: PropTypes.object.isRequired,
  isOrg: PropTypes.bool.isRequired,
  isDeliveryToUser: PropTypes.bool.isRequired,
};

export default ViewDeliveryOffcanvas;
