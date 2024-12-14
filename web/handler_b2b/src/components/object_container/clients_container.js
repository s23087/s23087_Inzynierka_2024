import PropTypes from "prop-types";
import { Container, Row, Col } from "react-bootstrap";
import ContainerButtons from "../smaller_components/container_buttons";

/**
 * Create element that represent client object
 * @component
 * @param {Object} props
 * @param {{clientId: Number, clientName: string, street: string, city: string, postal: string, nip: Number|undefined, country: string}} props.client Object value
 * @param {boolean} props.is_org True if org view is enabled
 * @param {boolean} props.selected True if container should show as selected
 * @param {Function} props.selectAction Action that will activated after clicking select button
 * @param {Function} props.unselectAction Action that will activated after clicking unselect button
 * @param {Function} props.deleteAction Action that will activated after clicking delete button
 * @param {Function} props.viewAction Action that will activated after clicking view button
 * @param {Function} props.modifyAction Action that will activated after clicking modify button
 * @return {JSX.Element} Container element
 */
function ClientContainer({
  client,
  is_org,
  selected,
  selectAction,
  unselectAction,
  deleteAction,
  viewAction,
  modifyAction,
}) {
  // Styles
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  /**
   * Build address string using client properties
   * @return {string} Return string representing address
   */
  const getAddress = () => {
    let tmp = client.street + " " + client.city + " " + client.postal;
    return tmp;
  };
  return (
    <Container
      className="py-3 px-4 px-xl-5 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" md="6" lg="6" xl="4">
          {is_org ? (
            <Row className="mb-2">
              <Col className="d-flex">
                <span className="me-2 mt-1 userIconStyle" title="user icon"/>
                <span className="spanStyle main-grey-bg d-flex rounded-span px-2 w-100 my-1">
                  <p className="mb-0 text-truncate d-block w-100">
                    {client.users.length > 0 ? client.users.join(", ") : "-"}
                  </p>
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
                <p className="mb-0 text-truncate d-block w-100">
                  Address: {getAddress()}
                </p>
              </span>
            </Col>
            <Col className="pe-1" xs="auto">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 d-md-none">
                <p className="mb-0">Nip: {client.nip ? client.nip : "-"}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 d-md-none">
                <p className="mb-0 text-truncate d-block w-100">
                  Country: {client.country}
                </p>
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
              selectAction();
            }}
            unselectAction={() => {
              unselectAction();
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

ClientContainer.propTypes = {
  client: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
  selectAction: PropTypes.func,
  unselectAction: PropTypes.func,
  deleteAction: PropTypes.func,
  viewAction: PropTypes.func,
  modifyAction: PropTypes.func,
};

export default ClientContainer;
