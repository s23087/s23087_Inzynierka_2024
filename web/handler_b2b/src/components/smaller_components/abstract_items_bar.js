import PropTypes from "prop-types";
import Link from "next/link";
import { Stack, Container, Button } from "react-bootstrap";

function AbstractItemPostionBar({ site_name }) {
  const barHeight = {
    height: "65px",
  };
  const buttonStyle = {
    height: "40px",
    width: "154px",
  };
  return (
    <Stack
      className="border-top-grey py-1 px-3"
      direction="horizontal"
      style={barHeight}
    >
      <Container className="me-auto ms-0 ms-xl-5">
        <p className="mb-0 blue-main-text">{site_name}</p>
      </Container>
      <Container className="w-auto me-0 me-xl-5">
        <Link href="settings">
          <Button variant="mainBlue" style={buttonStyle}>
            Go back
          </Button>
        </Link>
      </Container>
    </Stack>
  );
}

AbstractItemPostionBar.PropTypes = {
  site_name: PropTypes.string.isRequired,
};

export default AbstractItemPostionBar;
