import PropTypes from "prop-types";
import Image from "next/image";
import { Container, Row, Col } from "react-bootstrap";
import ContainerButtons from "../smaller_components/container_buttons";
import user_small_icon from "../../../public/icons/user_small_icon.png";

function ClientContainer({
  client,
  is_org,
  selected,
  selectQtyAction,
  unselectQtyAction,
  deleteAction,
  viewAction,
  modifyAction,
}) {
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  const getAddress = () => {
    let tmp = client.street + " " + client.city + " " + client.postal;
    if (tmp.length > 52) tmp = tmp.slice(0, 50) + "...";
    return tmp;
  };
  const getUsers = () => {
    if (!client.users) return "-";
    let tmp = client.users.join(", ");
    if (tmp.length > 52) tmp = tmp.slice(0, 50) + "...";
    return tmp;
  };
  return (
    <Container
      className="py-3 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" md="6" lg="6" xl="4">
          {is_org ? (
            <Row className="mb-2">
              <Col className="d-flex">
                <Image
                  src={user_small_icon}
                  alt="user small icon"
                  className="me-2 mt-1"
                />
                <span className="spanStyle main-grey-bg d-flex rounded-span px-2 w-100 my-1">
                  <p className="mb-0">{getUsers()}</p>
                </span>
              </Col>
            </Row>
          ) : null}
          <Row className="gy-2">
            <Col xs="12">
              <span className="spanStyle main-blue-bg main-text d-flex rounded-span px-2">
                <p className="mb-0">Name: {client.clientName}</p>
              </span>
            </Col>
            <Col xs="12">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">Address: {getAddress()}</p>
              </span>
            </Col>
            <Col className="pe-1" xs="auto">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 d-md-none">
                <p className="mb-0">Nip: {client.nip ? client.nip : "-"}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 d-md-none">
                <p className="mb-0">Country: {client.country}</p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col md="6" lg="6" xl="4" className="d-none d-md-block ps-xl-5">
          <Row className="gy-2 h-100 align-items-end">
            <Col xs="12">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">Nip: {client.nip}</p>
              </span>
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 mt-2">
                <p className="mb-0">Country: {client.country}</p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col xs="12" xl="4" className="px-0 pt-3 pt-xl-2 pb-2">
          <ContainerButtons
            selected={selected}
            selectAction={() => {
              selectQtyAction();
            }}
            unselectAction={() => {
              unselectQtyAction();
            }}
            deleteAction={deleteAction}
            viewAction={viewAction}
            modifyAction={modifyAction}
          />
        </Col>
      </Row>
    </Container>
  );
}

ClientContainer.PropTypes = {
  client: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
  selectQtyAction: PropTypes.func,
  unselectQtyAction: PropTypes.func,
  deleteAction: PropTypes.func,
  viewAction: PropTypes.func,
  modifyAction: PropTypes.func,
};

export default ClientContainer;
