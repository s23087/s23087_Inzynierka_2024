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
import validators from "@/utils/validators/validator";
import { useEffect, useState } from "react";
import SuccessFadeAway from "../smaller_components/success_fade_away";
import ErrorMessage from "../smaller_components/error_message";
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import getInvoiceItemForCredit from "@/utils/documents/get_invoice_item_for_credit";

/**
 * Modal element that allow to add products to credit note.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.modalShow Modal show parameter.
 * @param {Function} props.onHideFunction Function that will close modal (set modalShow to false).
 * @param {Function} props.addFunction Function that will activate after clicking add button.
 * @param {Array<{id: number, partnumber: string, name: string, qty: number, price: number, priceId: number, invoiceId: number, purchasePrice: number}>} props.addedProducts Array of already added object.
 * @param {Number} props.invoiceId Invoice id.
 * @param {boolean} props.isYourCredit If credit note type is "yours credit notes" then true, otherwise false.
 * @return {JSX.Element} Modal element
 */
function AddCreditProductWindow({
  modalShow,
  onHideFunction,
  addFunction,
  addedProducts,
  invoiceId,
  isYourCredit,
}) {
  // Product list
  const [products, setProducts] = useState([]);
  // Chosen product index
  const [currentProduct, setCurrentProduct] = useState(0);
  // True if download error occurred
  const [downloadError, setDownloadError] = useState([]);
  useEffect(() => {
    if (modalShow && addedProducts.length === 0) {
      getInvoiceItemForCredit(invoiceId, isYourCredit).then((data) => {
        if (data !== null) {
          setProducts(data);
          setDownloadError(false);
        } else {
          setDownloadError(true);
        }
      });
    }
    if (modalShow && addedProducts.length > 0) {
      getInvoiceItemForCredit(invoiceId, isYourCredit).then((data) => {
        if (data === null) {
          setDownloadError(true);
          return;
        }
        setDownloadError(false);
        filterQtyOfAddedProducts(data);
        setProducts(data);
      });
    }
  }, [modalShow, addedProducts.length]);
  // Errors
  const [priceError, setPriceError] = useState(false);
  const [qtyError, setQtyError] = useState(false);
  // True if you want to show success information to user
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
            <Form.Select
              className="input-style shadow-sm"
              id="product"
              key={products}
              onChange={(e) => setCurrentProduct(e.target.value)}
            >
              {products.length === 0 ? <option>Is loading...</option> : null}
              {products.map((val, key) => {
                if (val.qty > 0) {
                  return (
                    <option
                      key={[
                        val.itemId,
                        val.invoiceId,
                        val.priceId,
                        val.qty,
                        val.price,
                        key,
                      ]}
                      value={key}
                    >
                      {val.partnumber + " - " + val.qty + " pcs " + val.price}
                    </option>
                  );
                }
              })}
            </Form.Select>
          </Form.Group>
          <Form.Group className="mb-2">
            <Form.Label className="blue-main-text">Qty:</Form.Label>
            <ErrorMessage
              message={`Must be between -${products[currentProduct] ? products[currentProduct].qty : 0} and ${products[currentProduct] ? products[currentProduct].qty : 0}. Can not be 0.`}
              messageStatus={qtyError}
            />
            <Form.Control
              className="input-style shadow-sm"
              type="number"
              id="qty"
              isInvalid={qtyError}
              key={[currentProduct, products]}
              defaultValue={
                products[currentProduct] ? products[currentProduct].qty : 0
              }
              onInput={(e) => {
                if (!e.target.value) {
                  setQtyError(true);
                  return;
                }
                if (!validators.haveOnlyIntegers(e.target.value)) {
                  setQtyError(true);
                  return;
                }
                let parsedValue = parseInt(e.target.value);
                if (
                  parsedValue > products[currentProduct].qty ||
                  parsedValue < products[currentProduct].qty * -1 ||
                  parsedValue === 0
                ) {
                  setQtyError(true);
                } else {
                  setQtyError(false);
                }
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
              key={[currentProduct, products]}
              defaultValue={
                products[currentProduct]
                  ? products[currentProduct].price
                  : "Choose product"
              }
              isInvalid={priceError}
              onInput={(e) => {
                InputValidator.decimalValidator(e.target.value, setPriceError);
              }}
            />
          </Form.Group>
          <Stack className="px-3 mt-4" direction="horizontal">
            <Button
              variant="mainBlue"
              className="me-2 w-100"
              disabled={priceError || qtyError || downloadError}
              onClick={() => {
                setShowSuccess(false);
                if (products.filter((e) => e.qty > 0).length <= 0) return;
                if (priceError || qtyError || downloadError) {
                  return;
                }
                let productKey = document.getElementById("product").value;
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
                if (!qty || qty === "0") {
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

                let wholeProduct = products[productKey];
                products[productKey].qty -= Math.abs(parseInt(qty));
                if (products[productKey].qty === 0) {
                  let newIndex = products.findIndex((e) => e.qty > 0);
                  setCurrentProduct(newIndex === -1 ? 0 : newIndex);
                }
                addFunction({
                  id: wholeProduct.itemId,
                  partnumber: wholeProduct.partnumber,
                  name: wholeProduct.itemName,
                  qty: parseInt(qty),
                  price: parseFloat(price),
                  priceId: parseInt(wholeProduct.priceId),
                  invoiceId: parseInt(wholeProduct.invoiceId),
                  purchasePrice: wholeProduct.price,
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
              onClick={() => {
                let firstProduct = products.findIndex((e) => e.qty > 0);
                setPriceError(false);
                setQtyError(false);
                setCurrentProduct(firstProduct === -1 ? 0 : firstProduct);
                onHideFunction();
              }}
            >
              Close
            </Button>
          </Stack>
        </Container>
      </Modal.Body>
    </Modal>
  );

  /**
   * Reduce qty of item that already exist in added items. For example if user added 3 of 5 product A, then in will reduce to 2 free items in array.
   */
  function filterQtyOfAddedProducts(data) {
    data.forEach((element) => {
      let qtyToSubtract = addedProducts
        .filter((e) => e.id === element.itemId)
        .reduce((sum, item) => sum + Math.abs(item.qty), 0);
      if (qtyToSubtract !== 0) {
        element.qty -= qtyToSubtract;
      }
    });
  }
}

AddCreditProductWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  addFunction: PropTypes.func.isRequired,
  addedProducts: PropTypes.array.isRequired,
  invoiceId: PropTypes.number.isRequired,
  isYourCredit: PropTypes.bool.isRequired,
};

export default AddCreditProductWindow;
