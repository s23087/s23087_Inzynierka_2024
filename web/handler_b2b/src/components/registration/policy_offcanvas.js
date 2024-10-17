import { Container, Row, Col, Button, Offcanvas } from "react-bootstrap";
import Image from "next/image";
import PropTypes from "prop-types";
import dropdown from "../../../public/icons/dropdown_big_down.png";

function PolicyOffcanvas({ show, hideFunction }) {
  return (
    <Offcanvas
      className="h-100"
      show={show}
      onHide={hideFunction}
      placement="bottom"
    >
      <Offcanvas.Header className="border-bottom-grey">
        <Container fluid>
          <Row>
            <Col xs="9" className="d-flex align-items-center">
              <p className="blue-main-text h4 mb-0">About our policy</p>
            </Col>
            <Col xs="3" className="text-end pe-0">
              <Button variant="as-link" onClick={hideFunction} className="pe-0">
                <Image src={dropdown} alt="Dropdown" />
              </Button>
            </Col>
          </Row>
        </Container>
      </Offcanvas.Header>
      <Offcanvas.Body className="small-text px-4 mx-1">
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce
          consectetur sapien quam, in porttitor lectus convallis in. Nulla
          facilisi. Curabitur vitae justo lectus. Orci varius natoque penatibus
          et magnis dis parturient montes, nascetur ridiculus mus.
        </p>

        <p>
          Vestibulum volutpat ipsum eget condimentum iaculis. Nulla sodales,
          libero sit amet mollis sollicitudin, diam magna molestie lacus, non
          placerat ex ex accumsan risus. Donec condimentum neque id risus
          ullamcorper viverra. Nunc eget luctus massa. Integer at semper urna,
          vel pretium orci.
        </p>

        <p>
          Etiam nisl dolor, porttitor sed sem vel, laoreet tristique ipsum.
          Aenean a lorem placerat, condimentum eros euismod, mollis lectus.
          Etiam suscipit erat eu justo placerat tristique. Ut lectus ipsum,
          suscipit vitae lobortis nec, ornare id mauris. Vestibulum lectus
          velit, volutpat vel hendrerit quis, euismod nec lacus. Ut ut ultrices
          massa, eu sodales nunc.
        </p>

        <p>
          Nam sit amet risus rutrum eros faucibus pulvinar. In condimentum
          ultrices metus. Etiam malesuada leo a mi sollicitudin pretium. Vivamus
          ipsum lorem, porta aliquet augue eu, tincidunt aliquet massa. Sed sed
          molestie lacus. Sed blandit pretium velit et blandit.
        </p>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

PolicyOffcanvas.propTypes = {
  show: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
};

export default PolicyOffcanvas;
