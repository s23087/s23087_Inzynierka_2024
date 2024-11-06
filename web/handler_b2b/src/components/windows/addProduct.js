"use client";

import PropTypes from "prop-types";
import {
  Modal,
  Container,
  Row,
  Col,
  Form,
  Button,
  Stack,
} from "react-bootstrap";
import { useEffect, useState } from "react";
import getItemsList from "@/utils/documents/get_products";
import SuccessFadeAway from "../smaller_components/success_fade_away";
import ErrorMessage from "../smaller_components/error_message";
import InputValidator from "@/utils/validators/form_validator/inputValidator";

/**
 * Modal element that allow to add products to document.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.modalShow Modal show parameter.
 * @param {Function} props.onHideFunction Function that will close modal (set modalShow to false).
 * @param {Function} props.addFunction Function that will activate after clicking add button.
 * @return {JSX.Element} Modal element
 */
function AddProductWindow({ modalShow, onHideFunction, addFunction }) {
  // Products list
  const [products, setProducts] = useState([]);
  // True if download error happen
  const [downloadError, setDownloadError] = useState(false);
  useEffect(() => {
    if (modalShow) {
      getItemsList().then((data) => {
        if (data !== null) {
          setDownloadError(false);
          setProducts(data);
        } else {
          setDownloadError(true);
        }
      });
    }
  }, [modalShow]);
  // Error
  const [priceError, setPriceError] = useState(false);
  const [qtyError, setQtyError] = useState(false);
  // True if success info should be shown
  const [showSuccess, setShowSuccess] = useState(false);
  return (
    <Modal
      size="md"
      show={modalShow}
      centered
      className="px-4 minScalableWidth"
    >
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Add Product</h5>
            </Col>
            <SuccessFadeAway
              showSuccess={showSuccess}
              setShowSuccess={setShowSuccess}
            />
          </Row>
        </Container>
        <Container className="mt-3 mb-2">
          <ErrorMessage
            message="Could not download products."
            messageStatus={downloadError}
          />
          <Form.Group className="mb-3">
            <Form.Label className="blue-main-text">Product:</Form.Label>
            <Form.Select className="input-style shadow-sm" id="product">
              {products.map((val, key) => {
                return (
                  <option key={val.id} value={key}>
                    {val.partnumber}
                  </option>
                );
              })}
            </Form.Select>
          </Form.Group>
          <Form.Group className="mb-2">
            <Form.Label className="blue-main-text">Qty:</Form.Label>
            <ErrorMessage
              message="Cannot be empty or lower then 0."
              messageStatus={qtyError}
            />
            <Form.Control
              className="input-style shadow-sm"
              type="number"
              id="qty"
              isInvalid={qtyError}
              placeholder="qty"
              onInput={(e) => {
                InputValidator.onlyNumberValidator(e.target.value, setQtyError)
              }}
            />
          </Form.Group>
          <Form.Group className="mb-2">
            <Form.Label className="blue-main-text">Price:</Form.Label>
            <ErrorMessage
              message="Is empty or is not a number."
              messageStatus={priceError}
            />
            <Form.Control
              className="input-style shadow-sm maxInputWidth"
              type="text"
              id="price"
              placeholder="price"
              isInvalid={priceError}
              onInput={(e) => {
                InputValidator.decimalValidator(e.target.value, setPriceError)
              }}
            />
          </Form.Group>
          <Stack className="px-3 mt-4" direction="horizontal">
            <Button
              variant="mainBlue"
              className="me-2 w-100"
              disabled={
                getIsFormErrorActive()
              }
              onClick={() => {
                // Check for errors, then use add function
                setShowSuccess(false);
                if (
                  getIsFormErrorActive()
                ) {
                  return;
                }
                let product = document.getElementById("product").value;
                let qty = document.getElementById("qty").value;
                let price = document.getElementById("price").value;
                if (!price && !qty) {
                  setPriceError(true);
                  setQtyError(true);
                  return;
                } else {
                  setPriceError(false);
                  setQtyError(false);
                }
                if (!qty) {
                  setQtyError(true);
                  return;
                } else {
                  setQtyError(false);
                }
                if (!price) {
                  setPriceError(true);
                  return;
                } else {
                  setPriceError(false);
                }

                let wholeProduct = products[product];
                addFunction({
                  id: wholeProduct.id,
                  partnumber: wholeProduct.partnumber,
                  name: wholeProduct.name,
                  qty: parseInt(qty),
                  price: parseFloat(price),
                });
                if (!showSuccess) {
                  setShowSuccess(true);
                }
              }}
            >
              Add
            </Button>
            <Button
              variant="red"
              className="ms-2 w-100"
              onClick={() => onHideFunction()}
            >
              Close
            </Button>
          </Stack>
        </Container>
      </Modal.Body>
    </Modal>
  );

  /**
   * True if any of conditions are fulfilled, otherwise false
  */
  function getIsFormErrorActive() {
    return priceError || qtyError || downloadError || products.length === 0;
  }
}

AddProductWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  addFunction: PropTypes.func.isRequired,
};

export default AddProductWindow;
