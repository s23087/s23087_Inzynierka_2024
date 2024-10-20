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
import getItemsList from "@/utils/documents/get_products";
import SuccesFadeAway from "../smaller_components/succes_fade_away";
import ErrorMessage from "../smaller_components/error_message";

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
  const [downloadError, setDownloadError] = useState(false);
  useEffect(() => {
    if (modalShow && addedProductsQty === 0) {
      getItemsList(userId, currency).then((data) => {
        if (data !== null) {
          setDownloadError(false);
          setProducts(data);
        } else {
          setDownloadError(true);
        }
      });
    }
  }, [modalShow, addedProductsQty, userId, currency]);
  // Errors
  const [salesPriceError, setSalesPriceError] = useState(false);
  const [qtyError, setQtyError] = useState(false);
  // Misc
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
            <SuccesFadeAway
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
              onChange={(e) => setCurrentProduct(e.target.value)}
            >
              {products.map((val, key) => {
                if (val.qty > 0) {
                  return (
                    <option key={value} value={key}>
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
            <ErrorMessage
              message="Must not be bigger then chosen product qty. Must be a number."
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
            <ErrorMessage
              message="Is empty or is not a number."
              messageStatus={salesPriceError}
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
              isInvalid={salesPriceError}
              onInput={(e) => {
                if (
                  validators.isPriceFormat(e.target.value) &&
                  validators.stringIsNotEmpty(e.target.value)
                ) {
                  setSalesPriceError(false);
                } else {
                  setSalesPriceError(true);
                }
              }}
            />
          </Form.Group>
          <Stack className="px-3 mt-4" direction="horizontal">
            <Button
              variant="mainBlue"
              className="me-2 w-100"
              disabled={
                salesPriceError ||
                qtyError ||
                downloadError ||
                products.length === 0
              }
              onClick={() => {
                setShowSuccess(false);
                if (products.filter((e) => e.qty > 0).length <= 0) return;
                if (
                  salesPriceError ||
                  qtyError ||
                  downloadError ||
                  products.length === 0
                ) {
                  return;
                }
                let productKey = document.getElementById("product").value;
                let qty = document.getElementById("qty").value;
                let price = document.getElementById("price").value;
                if (!price && !qty) {
                  setSalesPriceError(true);
                  setQtyError(true);
                  return;
                } else {
                  setSalesPriceError(false);
                  setQtyError(false);
                }
                if (!qty) {
                  setQtyError(true);
                  return;
                } else {
                  setQtyError(false);
                }
                if (!price) {
                  setSalesPriceError(true);
                  return;
                } else {
                  setSalesPriceError(false);
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

AddSaleProductWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  addFunction: PropTypes.func.isRequired,
  userId: PropTypes.number.isRequired,
  currency: PropTypes.string.isRequired,
  addedProductsQty: PropTypes.number.isRequired,
};

export default AddSaleProductWindow;
