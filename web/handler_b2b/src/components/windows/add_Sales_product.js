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
import { useEffect, useRef, useState } from "react";
import getItemsList from "@/utils/documents/get_products";

function AddSaleProductWindow({
  modalShow,
  onHideFunction,
  addFunction,
  userId,
  currency,
  addedProductsQty,
}) {
  // Products
  const [products, setProducts] = useState([]);
  const [currentProduct, setCurrentProduct] = useState(0);
  useEffect(() => {
    if (modalShow && addedProductsQty === 0) {
      const products = getItemsList(userId, currency);
      products.then((data) => setProducts(data));
    }
  }, [modalShow]);
  // Errors
  const [priceError, setPriceError] = useState(false);
  const [qtyError, setQtyError] = useState(false);
  // Misc
  const [showSuccess, setShowSuccess] = useState(false);
  // styles
  const transition = {
    opacity: 0,
    transition: "all 250ms linear 1.5s",
  };
  const beforeTransition = {
    opacity: 100,
    transition: "all 200ms linear 0s",
  };
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Add Product</h5>
            </Col>
            <Col className="text-end">
              <p
                className="mb-0 mt-3 green-main-text"
                style={showSuccess ? beforeTransition : transition}
                onTransitionEnd={() => setShowSuccess(false)}
              >
                Success!
              </p>
            </Col>
          </Row>
        </Container>
        <Container className="mt-3 mb-2">
          <Form.Group className="mb-3">
            <Form.Label className="blue-main-text">Product:</Form.Label>
            <Form.Select
              className="input-style shadow-sm"
              id="product"
              onChange={(e) => setCurrentProduct(e.target.value)}
            >
              {products.map((val, key) => {
                if (val.qty > 0) {
                  return (
                    <option key={val.priceId} value={key}>
                      {val.partnumber +
                        " - " +
                        val.qty +
                        " pcs " +
                        val.price +
                        " " +
                        currency}
                    </option>
                  );
                }
              })}
            </Form.Select>
          </Form.Group>

          <Form.Group className="mb-2">
            <Form.Label className="blue-main-text">Qty:</Form.Label>
            <p
              className="text-start mb-1 red-sec-text small-text"
              style={qtyError ? unhidden : hidden}
            >
              Must not be bigger then chosen product qty. Must be a number.
            </p>
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
                if (
                  validators.haveOnlyNumbers(e.target.value) &&
                  validators.stringIsNotEmpty(e.target.value) &&
                  parseInt(e.target.value) > 0 &&
                  parseInt(e.target.value) <= products[currentProduct].qty
                ) {
                  setQtyError(false);
                } else {
                  setQtyError(true);
                }
              }}
            />
          </Form.Group>
          <Form.Group className="mb-2">
            <Form.Label className="blue-main-text">Price:</Form.Label>
            <p
              className="text-start mb-1 red-sec-text small-text"
              style={priceError ? unhidden : hidden}
            >
              Is empty or is not a number.
            </p>
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
                if (
                  validators.isPriceFormat(e.target.value) &&
                  validators.stringIsNotEmpty(e.target.value)
                ) {
                  setPriceError(false);
                } else {
                  setPriceError(true);
                }
              }}
            />
          </Form.Group>
          <Stack className="px-3 mt-4" direction="horizontal">
            <Button
              variant="mainBlue"
              className="me-2 w-100"
              onClick={() => {
                setShowSuccess(false);
                if (products.filter((e) => e.qty > 0).length <= 0) return;
                if (priceError || qtyError) {
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

                let wholeProduct = products[productKey];
                products[productKey].qty -= parseInt(qty);
                if (products[productKey].qty === 0) {
                  let newIndex = products.findIndex((e) => e.qty > 0);
                  setCurrentProduct(newIndex === -1 ? 0 : newIndex);
                }
                addFunction({
                  id: wholeProduct.itemId,
                  partnumber: wholeProduct.partnumber,
                  name: wholeProduct.name,
                  qty: parseInt(qty),
                  price: parseFloat(price),
                  priceId: parseInt(wholeProduct.priceId),
                  invoiceId: parseInt(wholeProduct.invoiceId),
                  invoiceNumber: wholeProduct.invoiceNumber,
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
              onClick={() => onHideFunction()}
            >
              Close
            </Button>
          </Stack>
        </Container>
      </Modal.Body>
    </Modal>
  );
}

AddSaleProductWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  statuses: PropTypes.object.isRequired,
  addFunction: PropTypes.func.isRequired,
  userId: PropTypes.number.isRequired,
  currency: PropTypes.string.isRequired,
  addedProductsQty: PropTypes.number.isRequired,
};

export default AddSaleProductWindow;
