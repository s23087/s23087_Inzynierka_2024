import PropTypes from "prop-types";
import { Stack, Container } from "react-bootstrap";
import OrgSwitcher from "../switches/org_switch";

function PagePostionBar({ site_name, with_switch, switch_bool }) {
  const barHeight = {
    height: "60px",
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
        {with_switch === true ? <OrgSwitcher is_org={switch_bool} /> : null}
      </Container>
    </Stack>
  );
}

PagePostionBar.PropTypes = {
  site_name: PropTypes.string.isRequired,
  with_switch: PropTypes.bool.isRequired,
  switch_bool: PropTypes.bool.isRequired,
};

export default PagePostionBar;
