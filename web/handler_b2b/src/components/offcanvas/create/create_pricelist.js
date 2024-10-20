import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import CloseIcon from "../../../../public/icons/close_black.png";
import ProductHolder from "@/components/smaller_components/product_holder";
import ErrorMessage from "@/components/smaller_components/error_message";
import InputValidtor from "@/utils/validators/form_validator/inputValidator";
import getOfferStatuses from "@/utils/pricelist/get_statuses";
import AddItemToPricelistWindow from "@/components/windows/add_item_to_pricelist";
import createPricelist from "@/utils/pricelist/create_pricelist";

function AddPricelistOffcanvas({ showOffcanvas, hideFunction }) {
  const router = useRouter();
  const [statusDownloadError, setStatusDownloadError] = useState(false);
  useEffect(() => {
    if (showOffcanvas) {
      getOfferStatuses().then((data) => {
        if (data === null) {
          setStatusDownloadError(true);
        } else {
          setStatusDownloadError(false);
          setStatuses(data);
        }
      });
    }
  }, [showOffcanvas]);
  const [statuses, setStatuses] = useState([]);
  // products
  const [showSalesProductWindow, setShowSalesProductWindow] = useState(false);
  const [products, setProducts] = useState([]);
  const [resetSeed, setResetSeed] = useState(false);
  // currency
  const [chosenCurrency, setChosenCurrency] = useState("PLN");
  // Errors
  const [nameError, setNameError] = useState(false);
  const [maxQtyError, setMaxQtyError] = useState(false);
  const isFormErrorActive = () =>
    nameError ||
    maxQtyError ||
    statuses.length === 0 ||
    products.length === 0 ||
    statusDownloadError;
  // Misc
  const [isLoading, setIsLoading] = useState(false);
  // Form
  const [state, formAction] = useFormState(
    createPricelist.bind(null, products).bind(null, chosenCurrency),
    {
      error: false,
      completed: false,
      message: "",
    },
  );
  // Styles
  const maxStyle = {
    maxWidth: "393px",
  };
  const vhStyle = {
    height: "81vh",
  };
  const buttonStyle = {
    maxWidth: "250px",
    minWidth: "200px",
    borderRadius: "5px",
  };
  const maxHeightScrollContainer = {
    maxHeight: "200px",
  };
  return (
    <Offcanvas
      className="h-100 minScalableWidth"
      show={showOffcanvas}
      onHide={hideFunction}
      placement="bottom"
    >
      <Container className="h-100 w-100 p-0" fluid>
        <Offcanvas.Header className="border-bottom-grey px-xl-5">
          <Container className="px-3" fluid>
            <Row>
              <Col xs="9" className="d-flex align-items-center">
                <p className="blue-main-text h4 mb-0">Create pricelist</p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                    setProducts([]);
                    setChosenCurrency("PLN");
                    if (!state.error && state.complete) {
                      router.refresh();
                    }
                  }}
                  className="pe-0"
                >
                  <Image src={CloseIcon} alt="Close" />
                </Button>
              </Col>
            </Row>
          </Container>
        </Offcanvas.Header>
        <Offcanvas.Body className="px-4 px-xl-5 pb-0" as="div">
          <Container className="p-0" style={vhStyle} fluid>
            <Form className="mx-1 mx-xl-3" id="offerForm" action={formAction}>
              <ErrorMessage
                message="Could not download the statuses."
                messageStatus={statusDownloadError}
              />
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Offer name:</Form.Label>
                <ErrorMessage
                  message="Is empty or lenght is greater than 100."
                  messageStatus={nameError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="name"
                  placeholder="offer name"
                  isInvalid={nameError}
                  onInput={(e) => {
                    InputValidtor.normalStringValidtor(
                      e.target.value,
                      setNameError,
                      100,
                    );
                  }}
                  maxLength={100}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Offer status:
                </Form.Label>
                <Form.Select
                  className="input-style shadow-sm maxInputWidth"
                  name="status"
                >
                  {statuses.map((val) => {
                    return (
                      <option key={val.statusId} value={val.statusId}>
                        {val.statusName}
                      </option>
                    );
                  })}
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Max showed product qty:
                </Form.Label>
                <ErrorMessage
                  message="Is empty or is not a number."
                  messageStatus={maxQtyError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="maxQty"
                  placeholder="qty"
                  isInvalid={maxQtyError}
                  onInput={(e) => {
                    InputValidtor.decimalValidator(
                      e.target.value,
                      setMaxQtyError,
                    );
                  }}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Currency:</Form.Label>
                <Form.Select
                  className="input-style shadow-sm maxInputWidth"
                  name="currency"
                  disabled={products.length > 0}
                  onChange={(e) => setChosenCurrency(e.target.value)}
                >
                  <option value="PLN">PLN</option>
                  <option value="EUR">EUR</option>
                  <option value="USD">USD</option>
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">Type:</Form.Label>
                <Form.Select
                  className="input-style shadow-sm maxInputWidth"
                  name="type"
                >
                  <option value="csv">csv</option>
                  <option value="xlsx">xml</option>
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-4 maxInputWidth">
                <Form.Label className="blue-main-text">Product:</Form.Label>
                <Container
                  className="overflow-y-scroll px-0"
                  style={maxHeightScrollContainer}
                  key={resetSeed}
                >
                  {products.map((value, key) => {
                    return (
                      <ProductHolder
                        key={value}
                        value={value}
                        deleteValue={() => {
                          products.splice(key, 1);
                          setResetSeed(!resetSeed);
                        }}
                      />
                    );
                  })}
                </Container>
                <Button
                  variant="mainBlue"
                  className="mb-3 mt-4 ms-3 py-3"
                  style={buttonStyle}
                  onClick={() => {
                    setShowSalesProductWindow(true);
                  }}
                >
                  Add Product
                </Button>
              </Form.Group>
              <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
                <Row style={maxStyle} className="mx-auto minScalableWidth">
                  <Col>
                    <Button
                      variant="mainBlue"
                      className="w-100"
                      type="submit"
                      disabled={isFormErrorActive()}
                      onClick={(e) => {
                        e.preventDefault();
                        setIsLoading(true);

                        let form = document.getElementById("offerForm");
                        form.requestSubmit();
                      }}
                    >
                      {isLoading && !state.completed ? (
                        <div className="spinner-border main-text"></div>
                      ) : (
                        "Create"
                      )}
                    </Button>
                  </Col>
                  <Col>
                    <Button
                      variant="red"
                      className="w-100"
                      onClick={() => {
                        hideFunction();
                        setProducts([]);
                        if (!state.error && state.completed) {
                          router.refresh();
                        }
                      }}
                    >
                      Cancel
                    </Button>
                  </Col>
                </Row>
              </Container>
            </Form>
          </Container>
        </Offcanvas.Body>
        <AddItemToPricelistWindow
          modalShow={showSalesProductWindow}
          onHideFunction={() => setShowSalesProductWindow(false)}
          addFunction={(val) => {
            products.push(val);
          }}
          currency={chosenCurrency}
          addedProducts={products}
        />
        <Toastes.ErrorToast
          showToast={state.completed && state.error}
          message={state.message}
          onHideFun={() => {
            resetState();
          }}
        />
        <Toastes.SuccessToast
          showToast={state.completed && !state.error}
          message={state.message}
          onHideFun={() => {
            resetState();
            document.getElementById("offerForm").reset();
            setProducts([]);
            setChosenCurrency("PLN");
            hideFunction();
            router.refresh();
          }}
        />
      </Container>
    </Offcanvas>
  );

  function resetState() {
    state.error = false;
    state.completed = false;
    state.message = "";
    setIsLoading(false);
  }
}

AddPricelistOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
};

export default AddPricelistOffcanvas;
