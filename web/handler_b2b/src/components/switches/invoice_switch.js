"use client";

import propTypes from "prop-types";
import { useState, useRef, useEffect } from "react";
import Image from "next/image";
import { Dropdown, Stack, Button } from "react-bootstrap";
import credit_note_icon from "../../../public/icons/credit_note_icon.png";
import request_icon from "../../../public/icons/request_icon.png";
import sales_icon from "../../../public/icons/sales_icon.png";
import yours_credit_note_icon from "../../../public/icons/yours_credit_note_icon.png";
import yours_invoice_icon from "../../../public/icons/yours_invoice_icon.png";

function getIcon(type) {
  switch (type) {
    case "Client credit note":
      return <Image src={credit_note_icon} alt="Document switch button" />;
    case "Yours credit note":
      return (
        <Image src={yours_credit_note_icon} alt="Document switch button" />
      );
    case "Request":
      return <Image src={request_icon} alt="Document switch button" />;
    case "Sales invoice":
      return <Image src={sales_icon} alt="Document switch button" />;
  }
  return <Image src={yours_invoice_icon} alt="Document switch button" />;
}

function InvoiceSwitch({ type, switch_action, is_role_solo }) {
  const doc_type = is_role_solo
    ? [
        "Yours invoices",
        "Sales invoices",
        "Yours credit notes",
        "Client credit notes",
      ]
    : [
        "Yours invoices",
        "Sales invoices",
        "Yours credit notes",
        "Client credit notes",
        "Requests",
      ];
  const menuRef = useRef(null);
  const onOutside = (event) => {
    if (menuRef.current && !menuRef.current.contains(event.target)) {
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
  return (
    <Dropdown className="mx-auto" drop="down-centered" show={closingBool}>
      <Dropdown.Toggle
        className="p-0 d-flex overflow-x-hidden toggle-style"
        variant="as-link"
        onClick={openSwitch}
      >
        {getIcon(type)}
      </Dropdown.Toggle>
      <Dropdown.Menu ref={menuRef}>
        <Stack gap={1} className="px-3 blue-main-text my-1">
          {doc_type.map((element) => {
            return element === type ? (
              <p className="my-2 h-100 blue-sec-text" key={element}>
                {element}
              </p>
            ) : (
              <Button
                variant="as-link"
                className="p-0 text-start"
                onClick={() => {
                  switch_action(element);
                  closeSwitch();
                }}
                key={element}
              >
                {element}
              </Button>
            );
          })}
        </Stack>
      </Dropdown.Menu>
    </Dropdown>
  );
}

getIcon.propTypes = {
  type: propTypes.bool.isRequired,
};

InvoiceSwitch.propTypes = {
  type: propTypes.bool.isRequired, // from switch statment in function 'getIcon'
  switch_action: propTypes.func.isRequired,
  is_role_solo: propTypes.bool.isRequired,
};

export default InvoiceSwitch;
