import PropTypes from "prop-types";
import { Button, InputGroup, Form } from "react-bootstrap";
import Image from "next/image";
import close_white from "../../../public/icons/close_white.png";

/**
 * Return input element that contain item information with delete option.
 * @component
 * @param {object} props Component props
 * @param {{partnumber: string, qty: Number, invoiceNumber: string|undefined, purchasePrice: Number, price: Number, name: string}} props.value Object that describes item.
 * @param {Function} props.deleteValue Function that will allow to delete value in array.
 * @return {JSX.Element} InputGroup element
 */
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
  /**
   * Calculate row number in input depend on how much information in object is available.
   * @return {Number}
   */
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

ProductHolder.propTypes = {
  value: PropTypes.string.isRequired,
  deleteValue: PropTypes.func.isRequired,
};

export default ProductHolder;
