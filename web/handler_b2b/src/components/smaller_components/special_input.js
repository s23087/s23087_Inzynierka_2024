import PropTypes from "prop-types";
import { useState } from "react";
import { Button, InputGroup, Form } from "react-bootstrap";
import Image from "next/image";
import validators from "@/utils/validators/validator";
import pen_icon from "../../../public/icons/pen_icon.png";
import close_white from "../../../public/icons/close_white.png";

function SpecialInput({ value, deleteValue, modifyValue, eanExistFun }) {
  const [isUnavail, setIsUnavail] = useState(true);
  const [ean, setEan] = useState(value);
  const [isInvalid, setIsInvalid] = useState(false);
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };
  return (
    <>
      <Form.Label
        className="text-start mb-0 red-sec-text small-text"
        style={isInvalid ? unhidden : hidden}
      >
        Ean already exist or have letters
      </Form.Label>
      <InputGroup className="mb-3 maxInputWidth">
        <Form.Control
          className="input-style shadow-sm"
          type="text"
          defaultValue={value}
          disabled={isUnavail}
          isInvalid={isInvalid}
          onInput={(e) => {
            if (
              validators.haveOnlyNumbers(e.target.value) &&
              (!eanExistFun(e.target.value) ||
                e.target.value == e.target.defaultValue)
            ) {
              setIsInvalid(false);
            } else {
              setIsInvalid(true);
            }
            setEan(e.target.value);
          }}
        />
        <Button variant="red" className="p-0" onClick={deleteValue}>
          <Image src={close_white} alt="close" />
        </Button>
        <Button
          variant={isUnavail ? "mainBlue" : "green"}
          className="p-0"
          onClick={() => {
            if (isInvalid) {
              return;
            }
            if (isUnavail) {
              setIsUnavail(false);
            } else {
              setIsUnavail(true);
            }
            if (!isUnavail) {
              modifyValue(ean);
            }
          }}
        >
          <Image src={pen_icon} alt="modify" />
        </Button>
      </InputGroup>
    </>
  );
}

SpecialInput.PropTypes = {
  value: PropTypes.string.isRequired,
  deleteValue: PropTypes.func.isRequired,
  modifyValue: PropTypes.func.isRequired,
  eanExistFun: PropTypes.func.isRequired,
};

export default SpecialInput;
