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
import SuccessFadeAway from "../smaller_components/success_fade_away";
import ErrorMessage from "../smaller_components/error_message";
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import getItemsForPricelist from "@/utils/pricelist/get_items_for_pricelist";
import validators from "@/utils/validators/validator";

/**
 * Modal element that allow to add products owned by chosen user to pricelist.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.modalShow Modal show parameter.
 * @param {Function} props.onHideFunction Function that will close modal (set modalShow to false).
 * @param {Function} props.addFunction Function that will activate after clicking add button.
 * @param {string} props.currency Shortcut of chosen by user currency name.
 * @param {Array<{ id: number, partnumber: string, qty: number, price: number, purchasePrice: number}>} props.addedProducts Array of already added object.
 * @return {JSX.Element} Modal element
 */
function AddItemToPricelistWindow({
  modalShow,
  onHideFunction,
  addFunction,
  currency,
  addedProducts,
}) {
  // Product list
  const [products, setProducts] = useState([]);
  // Chosen product index
  const [currentProduct, setCurrentProduct] = useState(0);
  useEffect(() => {
    if (modalShow) {
      getItemsForPricelist(currency).then((data) => {
        if (data === null) {
          setDownloadError(true);
        } else {
          setDownloadError(false);
          let newData = getFilteredData(data);
          setProducts(newData);
          if (newData[0]) {
            setChosenPrice(newData[0].purchasePrice.toFixed(2));
            setChosenMargin((0.0).toFixed(2));
          }
        }
      });
    }
    if (products[0] && modalShow) {
      setChosenPrice(products[0].purchasePrice.toFixed(2));
      setChosenMargin((0.0).toFixed(2));
    }
  }, [modalShow, addedProducts, currency]);
  const [chosenPrice, setChosenPrice] = useState(0);
  const [chosenMargin, setChosenMargin] = useState(0);
  /**
   * Calculate product margin based on price input and product purchase price. Return string "Error" if product does not exist or is not a number.
   * @return {Object} Margin of product with 2 decimal places or string error if occurred.
   */
  let getMargin = () => {
    if (isNaN(parseFloat(chosenPrice)) || !products[currentProduct]) {
      return "Error";
    } else {
      let result =
        ((chosenPrice - products[currentProduct].purchasePrice.toFixed(2)) /
          products[currentProduct].purchasePrice) *
        100;
      return result.toFixed(2);
    }
  };
  // Errors
  const [downloadError, setDownloadError] = useState(false);
  const [salesPriceError, setSalesPriceError] = useState(false);
  const [marginError, setMarginError] = useState(false);
  // Set true if action was a success and set false to hide success information
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
              onChange={(e) => {
                setCurrentProduct(e.target.value);
                setChosenPrice(
                  products[e.target.value].purchasePrice.toFixed(2),
                );
                setChosenMargin(getMargin());
              }}
            >
              {products.map((val, key) => {
                if (val.qty > 0) {
                  return (
                    <option key={key} value={key}>
                      {val.partnumber +
                        " - " +
                        val.qty +
                        " pcs " +
                        val.purchasePrice.toFixed(2) +
                        " " +
                        currency}
                    </option>
                  );
                }
              })}
            </Form.Select>
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
              key={[products, chosenPrice]}
              defaultValue={chosenPrice}
              isInvalid={salesPriceError}
              onInput={(e) => {
                InputValidator.decimalValidator(
                  e.target.value,
                  setSalesPriceError,
                );
                if (!e.target.value) return;
                if (validators.isPriceFormat(e.target.value)) {
                  let newMargin =
                    ((parseFloat(e.target.value) -
                      products[currentProduct].purchasePrice) /
                      products[currentProduct].purchasePrice) *
                    100;
                  setChosenMargin(newMargin.toFixed(2));
                }
              }}
            />
          </Form.Group>
          <Form.Group className="mb-2">
            <Form.Label className="blue-main-text">Margin:</Form.Label>
            <ErrorMessage
              message="Is empty or is not a number."
              messageStatus={marginError}
            />
            <Form.Control
              className="input-style shadow-sm maxInputWidth"
              type="text"
              id="margin"
              key={chosenMargin}
              defaultValue={chosenMargin}
              isInvalid={marginError}
              onInput={(e) => {
                InputValidator.decimalValidator(e.target.value, setMarginError);
                if (!e.target.value) return;
                if (!isNaN(parseFloat(e.target.value))) {
                  setChosenPrice(
                    (
                      products[currentProduct].purchasePrice +
                      products[currentProduct].purchasePrice *
                        (parseFloat(e.target.value) / 100)
                    ).toFixed(2),
                  );
                }
              }}
            />
          </Form.Group>
          <Stack className="px-3 mt-4" direction="horizontal">
            <Button
              variant="mainBlue"
              className="me-2 w-100"
              disabled={salesPriceError || products.length === 0}
              onClick={() => {
                setShowSuccess(false);
                if (products.filter((e) => e.qty > 0).length <= 0) return;
                if (salesPriceError) {
                  return;
                }
                let productKey = parseInt(
                  document.getElementById("product").value,
                );
                let price = document.getElementById("price").value;
                if (!price) {
                  setSalesPriceError(true);
                  return;
                } else {
                  setSalesPriceError(false);
                }

                let wholeProduct = products[productKey];
                products.splice(productKey, 1);
                setCurrentProduct(0);
                if (products[0]) {
                  setChosenPrice(products[0].purchasePrice);
                } else {
                  setChosenPrice((0.0).toFixed(2));
                }
                setChosenMargin((0.0).toFixed(2));
                document.getElementById("margin").value = 0.0;
                addFunction({
                  id: wholeProduct.itemId,
                  partnumber: wholeProduct.partnumber,
                  qty: wholeProduct.qty,
                  price: parseFloat(price),
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

  /**
   * Drop product that are already added
   */
  function getFilteredData(data) {
    return data.filter(
      (e) =>
        Object.values(addedProducts).findIndex((x) => x.id == e.itemId) === -1,
    );
  }
}

AddItemToPricelistWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  addFunction: PropTypes.func.isRequired,
  currency: PropTypes.string.isRequired,
  addedProducts: PropTypes.object.isRequired,
};

export default AddItemToPricelistWindow;
