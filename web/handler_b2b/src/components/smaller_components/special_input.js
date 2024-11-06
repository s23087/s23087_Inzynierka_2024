import PropTypes from "prop-types";
import { useState } from "react";
import { Button, InputGroup, Form } from "react-bootstrap";
import Image from "next/image";
import pen_icon from "../../../public/icons/pen_icon.png";
import close_white from "../../../public/icons/close_white.png";
import ErrorMessage from "./error_message";

/**
 * Return input element that contain array value with buttons.
 * @component
 * @param {object} props Component props
 * @param {string} props.value Value of input.
 * @param {Function} props.deleteValue Function that will allow to delete value in array.
 * @param {Function} props.modifyValue Function that will allow to modify value in array.
 * @param {Function} props.existFun Function that will check if value already exist in array.
 * @param {Function} props.validatorFunc Function that validates input when modifying.
 * @param {string} props.errorMessage Error message when input does not pass validation.
 * @return {JSX.Element} 
 */
function SpecialInput({
  value,
  deleteValue,
  modifyValue,
  existFun,
  validatorFunc,
  errorMessage,
}) {
  // true if input should be unmodifiable
  const [isUnavail, setIsUnavail] = useState(true);
  const [ean, setEan] = useState(value);
  const [isInvalid, setIsInvalid] = useState(false);
  return (
    <>
      <ErrorMessage message={errorMessage} messageStatus={isInvalid} />
      <InputGroup className="mb-3 maxInputWidth">
        <Form.Control
          className="input-style shadow-sm"
          type="text"
          defaultValue={value}
          disabled={isUnavail}
          isInvalid={isInvalid}
          onInput={(e) => {
            if (
              validatorFunc(e.target.value) &&
              (!existFun(e.target.value) ||
                e.target.value === e.target.defaultValue)
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

SpecialInput.propTypes = {
  value: PropTypes.string.isRequired,
  deleteValue: PropTypes.func.isRequired,
  modifyValue: PropTypes.func.isRequired,
  existFun: PropTypes.func.isRequired,
  validatorFunc: PropTypes.func.isRequired,
  errorMessage: PropTypes.string.isRequired,
};

export default SpecialInput;
