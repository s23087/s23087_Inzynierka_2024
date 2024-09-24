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
import SuccesFadeAway from "../smaller_components/succes_fade_away";
import ErrorMessage from "../smaller_components/error_message";
import StringValidtor from "@/utils/validators/form_validator/stringValidator";
import getInvoiceItemForCredit from "@/utils/documents/get_invoice_item_for_credit";

function AddCreditProductWindow({
  modalShow,
  onHideFunction,
  addFunction,
  addedProducts,
  invoiceId,
  isYourCredit,
}) {
  // Products
  const [products, setProducts] = useState([]);
  const [currentProduct, setCurrentProduct] = useState(0);
  useEffect(() => {
    if (modalShow && addedProducts.length === 0) {
      const servProducts = getInvoiceItemForCredit(invoiceId, isYourCredit);
      servProducts.then((data) => setProducts(data));
    }
    if (modalShow && addedProducts.length > 0) {
      const servProducts = getInvoiceItemForCredit(invoiceId, isYourCredit);
      servProducts.then((data) => {
        data.forEach((element) => {
          let qtyToSunstract = addedProducts
            .filter((e) => e.id === element.itemId)
            .reduce((sum, item) => sum + Math.abs(item.qty), 0);
          if (qtyToSunstract !== 0) {
            element.qty -= qtyToSunstract;
          }
        });
        setProducts(data);
      });
    }
  }, [modalShow, addedProducts.length]);
  // Errors
  const [priceError, setPriceError] = useState(false);
  const [qtyError, setQtyError] = useState(false);
  // Misc
  const [showSuccess, setShowSuccess] = useState(false);
  return (
    <Modal size="sm" show={modalShow} centered className="px-4">
      <Modal.Body>
        <Container>
          <Row>
            <Col>
              <h5 className="mb-0 mt-3">Add Product</h5>
            </Col>
            <SuccesFadeAway
              showSuccess={showSuccess}
              setShowSuccess={setShowSuccess}
            />
          </Row>
        </Container>
        <Container className="mt-3 mb-2">
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
                    <option key={key} value={key}>
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
              message={`Must be bettwen -${products[currentProduct] ? products[currentProduct].qty : 0} and ${products[currentProduct] ? products[currentProduct].qty : 0}. Can not be 0.`}
              messageStatus={qtyError}
            />
            <Form.Control
              className="input-style shadow-sm"
              type="number"
              id="qty"
              isInvalid={qtyError}
              key={[currentProduct, products]}
              defaultValue={
                products[currentProduct] ? products[currentProduct].qty : 1
              }
              onInput={(e) => {
                if (
                  validators.haveOnlyIntegers(e.target.value) &&
                  validators.stringIsNotEmpty(e.target.value) &&
                  parseInt(e.target.value) >=
                    products[currentProduct].qty * -1 &&
                  parseInt(e.target.value) <= products[currentProduct].qty &&
                  e.target.value !== "0"
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
                StringValidtor.decimalValidator(e.target.value, setPriceError);
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
}

AddCreditProductWindow.PropTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  addFunction: PropTypes.func.isRequired,
  addedProducts: PropTypes.array.isRequired,
  invoiceId: PropTypes.number.isRequired,
};

export default AddCreditProductWindow;
