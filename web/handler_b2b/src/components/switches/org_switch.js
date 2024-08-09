import PropTypes from "prop-types";
import Image from "next/image";
import { Button } from "react-bootstrap";
import org_switch from "../../../public/switches/org_switch_org.png";
import solo_switch from "../../../public/switches/org_switch_solo.png";

function OrgSwitcher({ is_org }) {
  return (
    <Button variant="as-link" className="p-0">
      {is_org === "true" ? (
        <Image src={org_switch} alt="Org switch" />
      ) : (
        <Image src={solo_switch} alt="Org switch" />
      )}
    </Button>
  );
}

OrgSwitcher.PropTypes = {
  is_org: PropTypes.bool.isRequired,
};

export default OrgSwitcher;
