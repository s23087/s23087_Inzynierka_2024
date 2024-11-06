import PropTypes from "prop-types";
import Link from "next/link";
import { Stack, Container, Button } from "react-bootstrap";

/**
 * Create container with page title and button that's allow to go back to settings.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.site_name Current page title.
 * @return {JSX.Element} Stack element
 */
function AbstractItemPositionBar({ site_name }) {
  const barHeight = {
    height: "60px",
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
      <Container className="me-auto ms-0 ms-xl-3">
        <p className="mb-0 blue-main-text">{site_name}</p>
      </Container>
      <Container className="w-auto me-0 me-xl-3">
        <Link href="settings">
          <Button variant="mainBlue" style={buttonStyle}>
            Go back
          </Button>
        </Link>
      </Container>
    </Stack>
  );
}

AbstractItemPositionBar.propTypes = {
  site_name: PropTypes.string.isRequired,
};

export default AbstractItemPositionBar;
