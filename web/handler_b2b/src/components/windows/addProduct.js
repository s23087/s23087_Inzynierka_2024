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

function AddProductWindow({ modalShow, onHideFunction, addFunction }) {
  const [products, setProducts] = useState([]);
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
  // succes var
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
                if (
                  validators.haveOnlyNumbers(e.target.value) &&
                  validators.stringIsNotEmpty(e.target.value)
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
              placeholder="price"
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
              disabled={
                priceError || qtyError || downloadError || products.length === 0
              }
              onClick={() => {
                setShowSuccess(false);
                if (
                  priceError ||
                  qtyError ||
                  downloadError ||
                  products.length === 0
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
}

AddProductWindow.propTypes = {
  modalShow: PropTypes.bool.isRequired,
  onHideFunction: PropTypes.func.isRequired,
  addFunction: PropTypes.func.isRequired,
};

export default AddProductWindow;
