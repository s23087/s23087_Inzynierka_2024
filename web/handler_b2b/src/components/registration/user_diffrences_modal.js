import PropTypes from "prop-types";
import { Modal, Container, Row, Col } from "react-bootstrap";
import Image from "next/image";
import CloseIcon from "../../../public/icons/close_black.png";
import MoreIcon from "../../../public/icons/more.png";

/**
 * Modal element that show information about types of user available.
 * @component
 * @param {object} props Component props
 * @param {boolean} modalShow Modal show parameter.
 * @param {Function} onHideFunction Function that will close modal (set modalShow to false).
 * @return {JSX.Element} Modal element
 */
function UserDiffModal({ modalShow, onHideFunction }) {
  const modalContainerStyle = {
    height: "360px",
    fontSize: "14px",
  };

  return (
    <Modal
      show={modalShow}
      onHide={onHideFunction}
      size="lg"
      centered
      className="px-4"
    >
      <Modal.Body className="mx-1 pt-2">
        <Container className="mb-4">
          <Row>
            <Col className="mt-1">
              <h5 className="mb-0 mt-2">User types</h5>
            </Col>
            <Col className="d-flex justify-content-end pe-0">
              <Image
                src={CloseIcon}
                alt="Close icon"
                onClick={onHideFunction}
              />
            </Col>
          </Row>
        </Container>
        <Container
          className="overflow-y-scroll lh-sm"
          style={modalContainerStyle}
        >
          <h6 className="mb-1">Individual user:</h6>
          <p>
            Individual user type is for users that work solo. The services allow to control your warehouse the same as organization type, 
            but it do not have features that allow to communicate between organization employees.
          </p>

          <p>
            If you are not sure if individual type is for you, then do not worry. You can quickly switch to Organization type if individual type is not enough for you.
          </p>

          <h6 className="mb-1">Org user:</h6>
          <p>
            Organization type is for users that lead business focused on selling products in bulk and need tool for supervising their warehouse and employees. This services
            offer communication tools between employees and view on state of warehouse and deals of all employees.
          </p>

          <p>
            This option allows you for easier management of your employees, improving workflow and documenting actions that happen in system.
          </p>
        </Container>
      </Modal.Body>
      <Modal.Footer className="justify-content-center py-1">
        <Image src={MoreIcon} alt="Three gray dot" />
      </Modal.Footer>
    </Modal>
  );
}

UserDiffModal.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
};

export default UserDiffModal;
