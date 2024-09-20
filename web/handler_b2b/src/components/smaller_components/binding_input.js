import PropTypes from "prop-types";
import { Button, InputGroup, Form } from "react-bootstrap";
import Image from "next/image";
import pen_icon from "../../../public/icons/pen_icon.png";

function BindingInput({ value, modifyAction }) {
  let bindingValue =
    value.username +
    "\nQty: " +
    value.qty +
    "\nPrice: " +
    value.price +
    " " +
    value.currency +
    "\n" +
    value.invoiceNumber;
  const inputStyle = {
    resize: "none",
  };
  return (
    <InputGroup className="mb-3 maxInputWidth">
      <Form.Control
        className="input-style shadow-sm overflow-y-hidden"
        type="text"
        defaultValue={bindingValue}
        as="textarea"
        rows={4}
        readOnly
        disabled
        style={inputStyle}
      />
      <Button className="p-0" onClick={modifyAction}>
        <Image src={pen_icon} alt="modify" />
      </Button>
    </InputGroup>
  );
}

BindingInput.PropTypes = {
  value: PropTypes.string.isRequired,
  deleteValue: PropTypes.func.isRequired,
  modifyAction: PropTypes.func.isRequired,
};

export default BindingInput;
