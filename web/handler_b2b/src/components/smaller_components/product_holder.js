import PropTypes from "prop-types";
import { Button, InputGroup, Form } from "react-bootstrap";
import Image from "next/image";
import close_white from "../../../public/icons/close_white.png";

function ProductHolder({ value, deleteValue }) {
  let productValue =
    value.partnumber +
    "\nQty: " +
    value.qty +
    " pcs " +
    (value.invoiceNumber ? "\nInvoice: " + value.invoiceNumber : "") +
    "\nPrice: " +
    (value.purchasePrice
      ? value.purchasePrice + " -> " + value.price
      : value.price) +
    (value.name ? "\n" + value.name : "");
  const inputStyle = {
    resize: "none",
  };
  let getRowNumber = () => {
    let result = 5;
    if (!value.invoiceNumber) result--;
    if (!value.name) result--;
    return result;
  };
  return (
    <InputGroup className="mb-3 maxInputWidth">
      <Form.Control
        className="input-style shadow-sm overflow-y-hidden"
        type="text"
        defaultValue={productValue}
        as="textarea"
        rows={getRowNumber()}
        readOnly
        disabled
        style={inputStyle}
      />
      <Button variant="red" className="p-0" onClick={deleteValue}>
        <Image src={close_white} alt="close" />
      </Button>
    </InputGroup>
  );
}

ProductHolder.PropTypes = {
  value: PropTypes.string.isRequired,
  deleteValue: PropTypes.func.isRequired,
};

export default ProductHolder;
