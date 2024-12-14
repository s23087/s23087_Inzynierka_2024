import PropTypes from "prop-types";
import { Container, Row, Col } from "react-bootstrap";
import ContainerButtons from "@/components/smaller_components/container_buttons";
import getDocumentStatusStyle from "@/utils/documents/get_document_status_color";

/**
 * Create element that represent credit note object
 * @component
 * @param {Object} props
 * @param {{user: string|undefined, creditNoteId: Number, invoiceNumber: string, date: string, qty: Number, total: Number, clientName: string, inSystem: boolean, isPaid: boolean}} props.credit_note Object value
 * @param {boolean} props.is_org True if org view is enabled
 * @param {boolean} props.selected True if container should show as selected
 * @param {boolean} props.is_user_type True if current type is "Yours credit notes"
 * @param {Function} props.selectAction Action that will activated after clicking select button
 * @param {Function} props.unselectAction Action that will activated after clicking unselect button
 * @param {Function} props.deleteAction Action that will activated after clicking delete button
 * @param {Function} props.viewAction Action that will activated after clicking view button
 * @param {Function} props.modifyAction Action that will activated after clicking modify button
 * @return {JSX.Element} Container element
 */
function CreditNoteContainer({
  credit_note,
  is_org,
  selected,
  is_user_type,
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
  return (
    <Container
      className="py-3 px-4 px-xl-5 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" md="7" lg="7" xl="4">
          {is_org ? (
            <Row className="mb-2">
              <Col className="d-flex">
                <span className="me-2 mt-1 userIconStyle" title="user icon" />
                <span className="spanStyle main-grey-bg d-flex rounded-span px-2 w-100 my-1 text-truncate d-block">
                  <p className="mb-0">{credit_note.user}</p>
                </span>
              </Col>
            </Row>
          ) : null}
          <Row className="gy-2">
            <Col xs="12" className="mb-1 mb-sm-0">
              <span className="spanStyle main-blue-bg main-text d-flex rounded-span px-2 text-truncate d-block">
                <p className="mb-0">{credit_note.invoiceNumber}</p>
              </span>
            </Col>
            <Col xs="12" className="mb-1 mb-md-0 mt-1 mt-sm-2">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">
                  Date: {credit_note.date.substring(0, 10)}
                </p>
              </span>
            </Col>
            <Col className="pe-1 mb-1 d-md-none">
              <span
                className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center"
                style={selected ? null : containerBg}
              >
                <p className="mb-0">Items Quantity:</p>
                <p className="mb-0">{credit_note.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-1 d-md-none">
              <span
                className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center"
                style={selected ? null : containerBg}
              >
                <p className="mb-0">Total Value:</p>
                <p className="mb-0">{credit_note.total} PLN</p>
              </span>
            </Col>
            <Col xs="12">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 text-truncate d-block">
                <p className="mb-0">
                  {is_user_type ? "Source: " : "For: "}
                  {credit_note.clientName}
                </p>
              </span>
            </Col>
            <Col className="pe-1 d-xxl-none" xs="auto">
              <span
                className="spanStyle d-flex rounded-span px-2"
                style={getDocumentStatusStyle(
                  credit_note.inSystem ? "In system" : "Not in system",
                )}
              >
                <p className="mb-0">
                  {credit_note.inSystem ? "In system" : "Not in system"}
                </p>
              </span>
            </Col>
            <Col className="ps-1 d-xxl-none">
              <span
                className="spanStyle main-grey-bg d-flex rounded-span px-2"
                style={getDocumentStatusStyle(
                  credit_note.isPaid ? "Paid" : "Unpaid",
                )}
              >
                <p className="mb-0">
                  Status: {credit_note.isPaid ? "Paid" : "Unpaid"}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col md="5" lg="5" xl="4" className="d-none d-md-block">
          <Row className="maxContainerStyle h-100 mx-auto">
            <Col className="pe-1 mb-2 mt-auto">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Items Quantity:</p>
                <p className="mb-0">{credit_note.qty}</p>
              </span>
            </Col>
            <Col className="ps-1 mb-2 mt-auto">
              <span className="main-blue-bg d-block rounded-span px-2 pb-2 pt-1 main-text text-center">
                <p className="mb-0">Total Value:</p>
                <p className="mb-0">{credit_note.total} PLN</p>
              </span>
            </Col>
            <Col xs="12">
              <Row className="d-none d-xxl-flex">
                <Col className="pe-1" xs="auto">
                  <span
                    className="spanStyle d-flex rounded-span px-2"
                    style={getDocumentStatusStyle(
                      credit_note.inSystem ? "In system" : "Not in system",
                    )}
                  >
                    <p className="mb-0">
                      {credit_note.inSystem ? "In system" : "Not in system"}
                    </p>
                  </span>
                </Col>
                <Col className="ps-1">
                  <span
                    className="spanStyle main-grey-bg d-flex rounded-span px-2"
                    style={getDocumentStatusStyle(
                      credit_note.isPaid ? "Paid" : "Unpaid",
                    )}
                  >
                    <p className="mb-0">
                      Status: {credit_note.isPaid ? "Paid" : "Unpaid"}
                    </p>
                  </span>
                </Col>
              </Row>
            </Col>
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

CreditNoteContainer.propTypes = {
  credit_note: PropTypes.object.isRequired,
  is_org: PropTypes.bool.isRequired,
  selected: PropTypes.bool.isRequired,
  is_user_type: PropTypes.bool.isRequired, // If true is user invoice, if false is client invoice
  selectAction: PropTypes.func.isRequired,
  unselectAction: PropTypes.func.isRequired,
  deleteAction: PropTypes.func.isRequired,
  viewAction: PropTypes.func.isRequired,
  modifyAction: PropTypes.func.isRequired,
};

export default CreditNoteContainer;
