"use client";

import PropTypes from "prop-types";
import Image from "next/image";
import { Button } from "react-bootstrap";
import org_switch from "../../../public/switches/org_switch_org.png";
import solo_switch from "../../../public/switches/org_switch_solo.png";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

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
      <Image
        src={is_org === true ? org_switch : solo_switch}
        alt={is_org === true ? "Org switch" : "Solo switch"}
      />
    </Button>
  );
}

OrgSwitcher.PropTypes = {
  is_org: PropTypes.bool.isRequired,
};

export default OrgSwitcher;
