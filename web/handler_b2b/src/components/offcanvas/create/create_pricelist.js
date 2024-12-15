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
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import getOfferStatuses from "@/utils/pricelist/get_statuses";
import AddItemToPricelistWindow from "@/components/windows/add_item_to_pricelist";
import createPricelist from "@/utils/pricelist/create_pricelist";

/**
 * Create offcanvas that allow to create pricelist.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @return {JSX.Element} Offcanvas element
 */
function AddPricelistOffcanvas({ showOffcanvas, hideFunction }) {
  const router = useRouter();
  // download data holder
  const [statuses, setStatuses] = useState([]);
  // download error
  const [statusDownloadError, setStatusDownloadError] = useState(false);
  // download data
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
  // useState for showing add sales window
  const [showSalesProductWindow, setShowSalesProductWindow] = useState(false);
  // products holder
  const [products, setProducts] = useState([]);
  // key to rerender product element that holds products
  const [resetSeed, setResetSeed] = useState(false);
  // currency chosen by user
  const [chosenCurrency, setChosenCurrency] = useState("PLN");
  // Form errors
  const [nameError, setNameError] = useState(false);
  const [maxQtyError, setMaxQtyError] = useState(false);
  /**
   * Checks if form can be submitted
   */
  const isFormErrorActive = () =>
    nameError ||
    maxQtyError ||
    statuses.length === 0 ||
    products.length === 0 ||
    statusDownloadError;
  // True if create action is running
  const [isLoading, setIsLoading] = useState(false);
  // Form action
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
                    setProducts([]);
                    setChosenCurrency("PLN");
                    hideFunction();
                    if (!state.error && state.completed) {
                      router.refresh();
                    }
                    resetState();
                  }}
                  className="pe-0"
                >
                  <Image src={CloseIcon} alt="Close" />
                </Button>
              </Col>
            </Row>
          </Container>
        </Offcanvas.Header>
        <Offcanvas.Body className="px-4 px-xl-5 pb-0 scrollableHeight" as="div">
          <Container className="p-0 pb-5" fluid>
            <Form className="mx-1 mx-xl-3" id="offerForm" action={formAction}>
              <ErrorMessage
                message="Could not download the statuses."
                messageStatus={statusDownloadError}
              />
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Offer name:</Form.Label>
                <ErrorMessage
                  message="Is empty or length is greater than 100."
                  messageStatus={nameError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="name"
                  placeholder="offer name"
                  isInvalid={nameError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
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
                    InputValidator.decimalValidator(
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
                        key={key}
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
                <Row
                  style={maxStyle}
                  className="mx-auto minScalableWidth offcanvasButtonsStyle"
                >
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
                        setProducts([]);
                        hideFunction();
                        if (!state.error && state.completed) {
                          router.refresh();
                        }
                        resetState();
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

  /**
   * Reset form state
   */
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
