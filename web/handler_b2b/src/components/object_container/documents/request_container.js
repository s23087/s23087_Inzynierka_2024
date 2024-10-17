import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col } from "react-bootstrap";
import ContainerButtons from "@/components/smaller_components/container_buttons";
import user_small_icon from "../../../../public/icons/user_small_icon.png";

function RequestContainer({
  request,
  is_org,
  selected,
  selectAction,
  unselectAction,
  viewAction,
  deleteAction,
  modifyAction,
  completeAction,
  rejectAction,
}) {
  const statusColorMap = {
    Fulfilled: "var(--main-green)",
    "In progress": "var(--main-yellow)",
    "Request cancelled": "var(--sec-red)",
  };
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  const statusStyle = {
    backgroundColor: statusColorMap[request.status],
    color:
      statusColorMap[request.status] === "var(--sec-red)"
        ? "var(--text-main-color)"
        : "var(--text-black-color)",
  };
  return (
    <Container
      className="py-3 px-4 px-xl-5 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" md="12" lg="7" xl="5">
          <Row className="mb-2">
            <Col className="d-flex">
              <Image
                src={user_small_icon}
                alt="user small icon"
                className="me-2 mt-1"
              />
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 w-100 my-1">
                <p className="mb-0">{request.username}</p>
              </span>
            </Col>
          </Row>
          <Row className="mb-2">
            <Col xs="6" className="pe-1">
              <span className="spanStyle main-blue-bg main-text d-flex rounded-span px-2">
                <p className="mb-0">{request.objectType}</p>
              </span>
            </Col>
            <Col xs="6" className="ps-1">
              <span
                className="spanStyle d-flex rounded-span px-2"
                style={statusStyle}
              >
                <p className="mb-0">
                  Request:{" "}
                  {request.status === "Request cancelled"
                    ? "Rejected"
                    : request.status}
                </p>
              </span>
            </Col>
          </Row>
          <Row className="mb-2">
            <Col>
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">{request.title}</p>
              </span>
            </Col>
          </Row>
          <Row>
            <Col>
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">
                  {request.creationDate.replace("T", " ").substring(0, 19)}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col
          xs="12"
          lg="5"
          xl="4"
          className="px-0 pt-4 pt-xl-2 pb-2 ms-xl-auto"
        >
          <ContainerButtons
            selected={selected}
            is_request={is_org}
            selectAction={selectAction}
            unselectAction={unselectAction}
            viewAction={viewAction}
            deleteAction={deleteAction}
            modifyAction={modifyAction}
            completeAction={completeAction}
            rejectAction={rejectAction}
            completeUnaval={request.status === "Fulfilled"}
            rejectUnaval={request.status === "Request cancelled"}
          />
        </Col>
      </Row>
    </Container>
  );
}

RequestContainer.propTypes = {
  item: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
  selectAction: PropTypes.func.isRequired,
  unselectAction: PropTypes.func.isRequired,
  viewAction: PropTypes.func.isRequired,
  deleteAction: PropTypes.func.isRequired,
  modifyAction: PropTypes.func.isRequired,
  completeAction: PropTypes.func.isRequired,
  rejectAction: PropTypes.func.isRequired,
};

export default RequestContainer;
