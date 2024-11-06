import PropTypes from "prop-types";
import { Stack, Container } from "react-bootstrap";
import OrgSwitcher from "../switches/org_switch";

/**
 * Create Bar that holds page name and view switch if enabled.
 * @component
 * @param {object} props
 * @param {string} props.site_name Name of current page
 * @param {boolean} props.with_switch True if you want to view switch to be enabled
 * @param {boolean} props.switch_bool True if org view is enabled, false if otherwise
 * @return {JSX.Element} Stack element
 */
function PagePositionBar({ site_name, with_switch, switch_bool }) {
  // Styles
  const barHeight = {
    height: "60px",
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
        {with_switch === true ? <OrgSwitcher is_org={switch_bool} /> : null}
      </Container>
    </Stack>
  );
}

PagePositionBar.propTypes = {
  site_name: PropTypes.string.isRequired,
  with_switch: PropTypes.bool.isRequired,
  switch_bool: PropTypes.bool.isRequired,
};

export default PagePositionBar;
