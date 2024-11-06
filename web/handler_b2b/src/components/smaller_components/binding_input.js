import PropTypes from "prop-types";
import { Button, InputGroup, Form } from "react-bootstrap";
import Image from "next/image";
import pen_icon from "../../../public/icons/pen_icon.png";

/**
 * Return input element that contain binding information with button to modify.
 * @component
 * @param {object} props Component props
 * @param {{username: string, qty: Number, price: Number, currency: string, invoiceNumber: string}} props.value Object representing binding.
 * @param {Function} props.modifyAction Function that will allow to modify binding.
 * @return {JSX.Element} InputGroup element
 */
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

BindingInput.propTypes = {
  value: PropTypes.string.isRequired,
  modifyAction: PropTypes.func.isRequired,
};

export default BindingInput;
