"use client";

import propTypes from "prop-types";
import { useState, useRef, useEffect } from "react";
import Image from "next/image";
import { Dropdown, Stack, Button } from "react-bootstrap";
import to_client_delivery_icon from "../../../public/icons/to_client_delivery_icon.png";
import to_user_delivery_icon from "../../../public/icons/to_user_delivery_icon.png";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

function DeliverySwitch({ isDeliveryToUser }) {
  const deliverySwitchRef = useRef(null);
  const onOutside = (event) => {
    if (
      deliverySwitchRef.current &&
      !deliverySwitchRef.current.contains(event.target)
    ) {
      setClosingBool(false);
    }
  };
  useEffect(() => {
    document.addEventListener("mousedown", onOutside);
    return () => document.removeEventListener("mousedown", onOutside);
  });
  const [closingBool, setClosingBool] = useState(false);
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  const buttonAction = (type) => {
    const newParams = new URLSearchParams(params);
    newParams.set("deliveryType", type);
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
        {isDeliveryToUser ? (
          <Image src={to_user_delivery_icon} alt="delivery switch icon" />
        ) : (
          <Image src={to_client_delivery_icon} alt="delivery switch icon" />
        )}
      </Dropdown.Toggle>
      <Dropdown.Menu ref={deliverySwitchRef}>
        <Stack gap={1} className="px-3 blue-main-text">
          {isDeliveryToUser ? (
            <>
              <p className="mb-0 pt-2 h-100 blue-sec-text">User deliveries</p>
              <Button
                variant="as-link"
                className="p-0 text-start"
                onClick={() => buttonAction("Deliveries to clients")}
              >
                Client deliveries
              </Button>
            </>
          ) : (
            <>
              <Button
                variant="as-link"
                className="p-0 text-start"
                onClick={() => buttonAction("Deliveries to user")}
              >
                User deliveries
              </Button>
              <p className="mb-2 h-100 blue-sec-text">Client deliveries</p>
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
