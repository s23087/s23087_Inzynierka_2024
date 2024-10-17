import PropTypes from "prop-types";
import { Modal, Container, Row, Col } from "react-bootstrap";
import Image from "next/image";
import CloseIcon from "../../../public/icons/close_black.png";
import MoreIcon from "../../../public/icons/more.png";

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
            Cras mattis consectetur purus sit amet fermentum. Cras justo odio,
            dapibus ac facilisis in, egestas eget quam. Morbi leo risus, porta
            ac consectetur ac, vestibulum at eros.
          </p>

          <p>
            Cras mattis consectetur purus sit amet fermentum. Cras justo odio,
            dapibus ac facilisis in, egestas eget quam. Morbi leo risus, porta
            ac consectetur ac, vestibulum at eros.
          </p>

          <h6 className="mb-1">Org user:</h6>
          <p>
            Cras mattis consectetur purus sit amet fermentum. Cras justo odio,
            dapibus ac facilisis in, egestas eget quam. Morbi leo risus, porta
            ac consectetur ac, vestibulum at eros.
          </p>

          <p>
            Cras mattis consectetur purus sit amet fermentum. Cras justo odio,
            dapibus ac facilisis in, egestas eget quam. Morbi leo risus, porta
            ac consectetur ac, vestibulum at eros.
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
