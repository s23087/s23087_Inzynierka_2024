"use client";

import PropTypes from "prop-types";
import { useState, useRef, useEffect } from "react";
import Image from "next/image";
import { Dropdown, Stack, Button } from "react-bootstrap";
import your_proformas_icon from "../../../public/icons/your_proformas_icon.png";
import clients_proformas_icon from "../../../public/icons/clients_proformas_icon.png";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

/**
 * Create element that acts as switch. User can change the type of proformas that he wants to see. That element will use query parameter to return chosen type of proforma.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.isYourProforma True if current proforma view shows "your proformas", otherwise false.
 * @return {JSX.Element} Dropdown element
 */
function ProformaSwitch({ isYourProforma }) {
  // This element reference
  const menuRef = useRef(null);
  const [closingBool, setClosingBool] = useState(false);
  // Check if mouse is outside element and ref current property has been initialized with DOM node
  const onOutside = (event) => {
    if (menuRef.current && !menuRef.current.contains(event.target)) {
      setClosingBool(false);
    }
  };
  // Add listener to this element
  useEffect(() => {
    document.addEventListener("mousedown", onOutside);
    return () => document.removeEventListener("mousedown", onOutside);
  });
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  /**
   * Sets query parameter to chosen type
   * @param {string} type Name of proforma type
  */
  const buttonAction = (type) => {
    const newParams = new URLSearchParams(params);
    newParams.set("proformaType", type);
    router.replace(`${pathName}?${newParams}`);
    setClosingBool(false);
  };
  return (
    <Dropdown className="mx-auto" drop="down-centered" show={closingBool}>
      <Dropdown.Toggle
        className="p-0 d-flex overflow-x-hidden toggle-style"
        variant="as-link"
        onClick={() => setClosingBool(true)}
      >
        {isYourProforma ? (
          <Image src={your_proformas_icon} alt="proforma switch icon" />
        ) : (
          <Image src={clients_proformas_icon} alt="proforma switch icon" />
        )}
      </Dropdown.Toggle>
      <Dropdown.Menu ref={menuRef}>
        <Stack gap={1} className="px-3 blue-main-text">
          {isYourProforma ? (
            <>
              <p className="mb-0 pt-2 h-100 blue-sec-text">User proformas</p>
              <Button
                variant="as-link"
                className="p-0 text-start"
                onClick={() => buttonAction("Clients proformas")}
              >
                Client proformas
              </Button>
            </>
          ) : (
            <>
              <Button
                variant="as-link"
                className="p-0 text-start"
                onClick={() => buttonAction("Yours proformas")}
              >
                User proformas
              </Button>
              <p className="mb-2 h-100 blue-sec-text">Client proformas</p>
            </>
          )}
        </Stack>
      </Dropdown.Menu>
    </Dropdown>
  );
}

ProformaSwitch.propTypes = {
  isYourProforma: PropTypes.bool.isRequired, // 1 for user, 0 for client
};

export default ProformaSwitch;
