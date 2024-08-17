"use client";

import propTypes from "prop-types";
import { useState, useRef, useEffect } from "react";
import Image from "next/image";
import { Dropdown, Stack, Button } from "react-bootstrap";
import to_client_delivery_icon from "../../../public/icons/to_client_delivery_icon.png";
import to_user_delivery_icon from "../../../public/icons/to_user_delivery_icon.png";

function DeliverySwitch({ boolean_value, switch_action }) {
  const deliverySwitchRef = useRef(null);
  const onOutside = (event) => {
    if (deliverySwitchRef.current && !deliverySwitchRef.current.contains(event.target)) {
      closeSwitch();
    }
  };
  useEffect(() => {
    document.addEventListener("mousedown", onOutside);
    return () => document.removeEventListener("mousedown", onOutside);
  });
  const [closingBool, setClosingBool] = useState(false);
  const openSwitch = () => setClosingBool(true);
  const closeSwitch = () => setClosingBool(false);
  const buttonAction = () => {
    switch_action();
    closeSwitch();
  };
  return (
    <Dropdown className="mx-auto" drop="down-centered" show={closingBool}>
      <Dropdown.Toggle
        className="p-0 d-flex overflow-x-hidden toggle-style"
        variant="as-link"
        onClick={openSwitch}
      >
        {boolean_value ? (
          <Image src={to_client_delivery_icon} alt="delivery switch icon" />
        ) : (
          <Image src={to_user_delivery_icon} alt="delivery switch icon" />
        )}
      </Dropdown.Toggle>
      <Dropdown.Menu ref={deliverySwitchRef}>
        <Stack gap={1} className="px-3 blue-main-text">
          {boolean_value ? (
            <>
              <Button
                variant="as-link"
                className="p-0 text-start"
                onClick={buttonAction}
              >
                User deliveries
              </Button>
              <p className="mb-2 h-100 blue-sec-text">Client deliveries</p>
            </>
          ) : (
            <>
              <p className="mb-0 pt-2 h-100 blue-sec-text">User deliveries</p>
              <Button
                variant="as-link"
                className="p-0 text-start"
                onClick={buttonAction}
              >
                Client deliveries
              </Button>
            </>
          )}
        </Stack>
      </Dropdown.Menu>
    </Dropdown>
  );
}

DeliverySwitch.propTypes = {
  boolean_value: propTypes.bool.isRequired, // 1 for client, 0 for user
  switch_action: propTypes.func.isRequired,
};

export default DeliverySwitch;
