import Image from "next/image";
import Link from "next/link";
import { Button, Container, Row, Col } from "react-bootstrap";
import PropTypes from "prop-types";
import user_icon from "../../../public/icons/user_icon.png";
import org_user_icon from "../../../public/icons/org_user_icon.png";

/**
 * Return button that will push user to given link. Used for choosing type of registration user wants to perform.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.is_org True if you want to get org version of button, false if you want get solo user version of button
 * @param {string} props.link Link that will forward user to next page
 * @return {JSX.Element} Link element
 */
function UserChooser({ is_org, link }) {
  const buttonStyle = {
    width: "259px",
    height: "122px",
  };

  if (is_org) {
    return (
      <Link
        className="link-underline link-underline-opacity-0 px-0 h-100"
        href={{
          pathname: link,
          query: { is_org: true },
        }}
      >
        <Button variant="mainBlue" type="button" style={buttonStyle}>
          <Container className="text-center px-3" fluid>
            <Row className="px-0">
              <Col className="px-0">
                <Image src={org_user_icon} alt="Org user icon" />
              </Col>
              <Col className="d-flex align-items-center">
                <p className="mb-0 ms-3">Org User</p>
              </Col>
            </Row>
          </Container>
        </Button>
      </Link>
    );
  }

  return (
    <Link
      className="link-underline link-underline-opacity-0 px-0 h-100"
      href={{
        pathname: link,
        query: { is_org: false },
      }}
    >
      <Button variant="mainBlue" type="button" style={buttonStyle}>
        <Container className="text-center px-3" fluid>
          <Row className="px-0">
            <Col className="px-0 ms-2" xs="4">
              <Image src={user_icon} alt="User icon" />
            </Col>
            <Col className="d-flex align-items-center text-center">
              <p className="mb-0 w-100">Individual User</p>
            </Col>
          </Row>
        </Container>
      </Button>
    </Link>
  );
}

UserChooser.propTypes = {
  is_org: PropTypes.bool.isRequired,
  link: PropTypes.string.isRequired,
};

export default UserChooser;
