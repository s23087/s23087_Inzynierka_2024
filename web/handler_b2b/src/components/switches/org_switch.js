"use client";

import PropTypes from "prop-types";
import Image from "next/image";
import { Button } from "react-bootstrap";
import org_switch from "../../../public/switches/org_switch_org.png";
import solo_switch from "../../../public/switches/org_switch_solo.png";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

/**
 * Create element that acts as switch. User can change the type of view from normal to organization one.
 * That element will use query parameter to return chosen type of view, by setting param isOrg and page (set as page 1).
 * If isOrg is true then organization view should be returned.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.is_org
 * @return {JSX.Element} Button element
 */
function OrgSwitcher({ is_org }) {
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  const accessParams = new URLSearchParams(params);
  return (
    <Button
      variant="as-link"
      className="p-0"
      onClick={() => {
        is_org = is_org !== true;
        accessParams.set("page", 1);
        accessParams.set("isOrg", is_org);
        router.push(`${pathName}?${accessParams}`);
      }}
    >
      <span className={is_org === true ? "orgSwitchIconStyle" : "noOrgSwitchIconStyle"} title="view switch"/>
    </Button>
  );
}

OrgSwitcher.propTypes = {
  is_org: PropTypes.bool.isRequired,
};

export default OrgSwitcher;
