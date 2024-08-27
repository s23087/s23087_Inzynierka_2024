"use client";

import propTypes from "prop-types";
import { useState, useRef, useEffect } from "react";
import Image from "next/image";
import { Dropdown, Stack, Button } from "react-bootstrap";
import your_proformas_icon from "../../../public/icons/your_proformas_icon.png";
import clients_proformas_icon from "../../../public/icons/clients_proformas_icon.png";

function ProformaSwitch({ boolean_value, switch_action }) {
  const menuRef = useRef(null);
  const [closingBool, setClosingBool] = useState(false);
  const onOutside = (event) => {
    if (menuRef.current && !menuRef.current.contains(event.target)) {
      setClosingBool(false);
    }
  };
  useEffect(() => {
    document.addEventListener("mousedown", onOutside);
    return () => document.removeEventListener("mousedown", onOutside);
  });
  const buttonAction = () => {
    switch_action();
    setClosingBool(false);
  };
  return (
    <Dropdown className="mx-auto" drop="down-centered" show={closingBool}>
      <Dropdown.Toggle
        className="p-0 d-flex overflow-x-hidden toggle-style"
        variant="as-link"
        onClick={() => setClosingBool(true)}
      >
        {boolean_value ? (
          <Image src={clients_proformas_icon} alt="proforma switch icon" />
        ) : (
          <Image src={your_proformas_icon} alt="proforma switch icon" />
        )}
      </Dropdown.Toggle>
      <Dropdown.Menu ref={menuRef}>
        <Stack gap={1} className="px-3 blue-main-text">
          {boolean_value ? (
            <>
              <Button
                variant="as-link"
                className="p-0 text-start"
                onClick={buttonAction}
              >
                User proformas
              </Button>
              <p className="mb-2 h-100 blue-sec-text">Client proformas</p>
            </>
          ) : (
            <>
              <p className="mb-0 pt-2 h-100 blue-sec-text">User proformas</p>
              <Button
                variant="as-link"
                className="p-0 text-start"
                onClick={buttonAction}
              >
                Client proformas
              </Button>
            </>
          )}
        </Stack>
      </Dropdown.Menu>
    </Dropdown>
  );
}

ProformaSwitch.propTypes = {
  boolean_value: propTypes.bool.isRequired, // 1 for client, 0 for user
  switch_action: propTypes.func.isRequired,
};

export default ProformaSwitch;
