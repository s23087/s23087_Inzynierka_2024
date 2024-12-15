import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import getUsers from "@/utils/flexible/get_users";
import getOrgsList from "@/utils/documents/get_orgs_list";
import getTaxes from "@/utils/documents/get_taxes";
import getPaymentMethods from "@/utils/documents/get_payment_methods";
import getPaymentStatuses from "@/utils/documents/get_payment_statuses";
import CloseIcon from "../../../../../public/icons/close_black.png";
import ProductHolder from "@/components/smaller_components/product_holder";
import AddProductWindow from "@/components/windows/addProduct";
import CreatePurchaseInvoice from "@/utils/documents/create_purchase_invoice";
import getCurrencyValuesList from "@/utils/flexible/get_currency_values_list";
import AddSaleProductWindow from "@/components/windows/add_Sales_product";
import CreateSalesInvoice from "@/utils/documents/create_sales_invoice";
import ErrorMessage from "@/components/smaller_components/error_message";
import InputValidator from "@/utils/validators/form_validator/inputValidator";

/**
 * Create offcanvas that allow to create invoice.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {boolean} props.isYourInvoice If type equal to "Yours invoices" then true, otherwise false.
 * @return {JSX.Element} Offcanvas element
 */
function AddInvoiceOffcanvas({ showOffcanvas, hideFunction, isYourInvoice }) {
  const router = useRouter();
  // download data holders
  const [users, setUsers] = useState([]);
  const [orgs, setOrgs] = useState({
    restOrgs: [],
  });
  const [taxes, setTaxes] = useState([]);
  const [paymentMethods, setPaymentMethods] = useState([]);
  const [paymentStatuses, setPaymentStatuses] = useState([]);
  // download errors
  const [userDownloadError, setUserDownloadError] = useState(false);
  const [orgDownloadError, setOrgDownloadError] = useState(false);
  const [taxesDownloadError, setTaxesDownloadError] = useState(false);
  const [methodsDownloadError, setMethodsDownloadError] = useState(false);
  const [statusesDownloadError, setStatusesDownloadError] = useState(false);
  // download data
  useEffect(() => {
    if (showOffcanvas) {
      getUsers().then((data) => {
        if (data === null) {
          setUserDownloadError(true);
        } else {
          setUserDownloadError(false);
          setUsers(data);
          setChosenUser(data[0].idUser);
        }
      });

      getOrgsList().then((data) => {
        if (data !== null) {
          setOrgDownloadError(false);
          setOrgs(data);
        } else {
          setOrgDownloadError(true);
        }
      });

      getTaxes().then((data) => {
        if (data !== null) {
          setTaxes(data);
          setTaxesDownloadError(false);
        } else {
          setTaxesDownloadError(true);
        }
      });

      getPaymentMethods().then((data) => {
        if (data !== null) {
          setMethodsDownloadError(false);
          setPaymentMethods(data);
        } else {
          setMethodsDownloadError(true);
        }
      });

      getPaymentStatuses().then((data) => {
        if (data !== null) {
          setStatusesDownloadError(false);
          setPaymentStatuses(data);
        } else {
          setStatusesDownloadError(true);
        }
      });
    }
  }, [showOffcanvas]);
  // useState for showing add product window
  const [showProductWindow, setShowProductWindow] = useState(false);
  // useState for showing add sale product window
  const [showSalesProductWindow, setShowSalesProductWindow] = useState(false);
  // product holder
  const [products, setProducts] = useState([]);
  // key variable for rerender component holding product
  const [resetSeed, setResetSeed] = useState(false);
  // File holder
  const [file, setFile] = useState();
  // chosen user id holder
  const [chosenUser, setChosenUser] = useState();
  //useState for showing currency exchange input
  const [showCurrencyExchange, setShowCurrencyExchange] = useState(false);
  // currency chosen by user holder
  const [chosenCurrency, setChosenCurrency] = useState("PLN");
  // invoice date chosen by user holder
  const [invoiceDate, setInvoiceDate] = useState(
    new Date().toLocaleDateString("en-CA"),
  );
  // currency download data holder
  const [currencyList, setCurrencyList] = useState({
    error: false,
    message: "",
    rates: [],
  });
  // download currency data
  useEffect(() => {
    if (showCurrencyExchange) {
      if (Date.parse(invoiceDate) > Date.now()) {
        setCurrencyList({
          error: true,
          message: "There is no existing value for this invoice date.",
          rates: [],
        });
        return;
      }
      getCurrencyValuesList(
        chosenCurrency,
        invoiceDate,
        new Date().toLocaleDateString("en-CA"),
      ).then((data) => {
        setCurrencyList({
          error: data.error,
          rates: data.rates,
        });
      });
    }
  }, [showCurrencyExchange, invoiceDate, chosenCurrency]);
  // Form errors
  const [invoiceError, setInvoiceError] = useState(false);
  const [transportError, setTransportError] = useState(false);
  const [documentError, setDocumentError] = useState(false);
  const [dateError, setDateError] = useState(false);
  /**
   * Check if form can be submitted
   */
  const isFormErrorActive = () =>
    invoiceError ||
    transportError ||
    documentError ||
    currencyList.error ||
    dateError ||
    products.length === 0 ||
    orgs.restOrgs.length === 0 ||
    orgDownloadError ||
    userDownloadError ||
    taxesDownloadError ||
    methodsDownloadError ||
    statusesDownloadError;
  // True if create action is running
  const [isLoading, setIsLoading] = useState(false);
  // Form action
  const [state, formPurchaseAction] = useFormState(
    isYourInvoice
      ? CreatePurchaseInvoice.bind(null, products)
          .bind(null, file)
          .bind(null, orgs)
          .bind(null, chosenCurrency)
      : CreateSalesInvoice.bind(null, products)
          .bind(null, file)
          .bind(null, orgs)
          .bind(null, chosenCurrency),
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
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
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
                <p className="blue-main-text h4 mb-0">
                  Create {isYourInvoice ? "purchase" : "sales"} invoice
                </p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                    setProducts([]);
                    setShowCurrencyExchange(false);
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
          <Container className="p-0" fluid>
            <Form
              className="mx-1 mx-xl-3"
              id="invoiceForm"
              action={formPurchaseAction}
            >
              <ErrorMessage
                message="Error: could not download all necessary info."
                messageStatus={
                  orgDownloadError ||
                  userDownloadError ||
                  taxesDownloadError ||
                  methodsDownloadError ||
                  statusesDownloadError
                }
              />
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">User:</Form.Label>
                <Form.Select
                  id="userSelect"
                  className="input-style shadow-sm maxInputWidth"
                  name="user"
                  onChange={(e) => {
                    setChosenUser(e.target.value);
                  }}
                >
                  {Object.values(users).map((value) => {
                    return (
                      <option key={value.idUser} value={value.idUser}>
                        {value.username + " " + value.surname}
                      </option>
                    );
                  })}
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Invoice Number:
                </Form.Label>
                <ErrorMessage
                  message="Is empty, not a number or length is greater than 40."
                  messageStatus={invoiceError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="invoice"
                  placeholder="invoice"
                  isInvalid={invoiceError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setInvoiceError,
                      40,
                    );
                  }}
                  maxLength={40}
                />
              </Form.Group>
              <Container className="px-0 maxInputWidth ms-0">
                <Row
                  style={
                    new Date(invoiceDate).getDay() === 0 ||
                    new Date(invoiceDate).getDay() === 6
                      ? unhidden
                      : hidden
                  }
                >
                  <p className="mb-0 text-start mb-1 warning-text small-text">
                    Warning! The chosen day is not a week day, therefore
                    currency values will be taken from last friday.
                  </p>
                </Row>
                <Row className="justify-content-between">
                  <Col xs="6" sm="4">
                    <Form.Group className="mb-4">
                      <Form.Label className="blue-main-text">Date:</Form.Label>
                      <Form.Control
                        className="input-style shadow-sm maxInputWidth"
                        type="date"
                        name="date"
                        isInvalid={dateError}
                        readOnly={products.length > 0}
                        defaultValue={new Date().toLocaleDateString("en-CA")}
                        onChange={(e) => {
                          let date = new Date(e.target.value);
                          if (date > Date.now()) {
                            setDateError(true);
                          } else {
                            setDateError(false);
                          }
                          setInvoiceDate(e.target.value);
                        }}
                      />
                      <ErrorMessage
                        message="Date exceed today's date."
                        messageStatus={dateError}
                      />
                    </Form.Group>
                  </Col>
                  <Col xs="6" sm="4">
                    <Form.Group className="mb-4">
                      <Form.Label className="blue-main-text">
                        Due date:
                      </Form.Label>
                      <Form.Control
                        className="input-style shadow-sm maxInputWidth"
                        type="date"
                        name="dueDate"
                        defaultValue={new Date().toLocaleDateString("en-CA")}
                      />
                    </Form.Group>
                  </Col>
                </Row>
              </Container>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  {isYourInvoice ? "Buyer:" : "Seller:"}
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  defaultValue={orgs.orgName}
                  readOnly
                  disabled
                />
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">
                  {isYourInvoice ? "Seller:" : "Buyer:"}
                </Form.Label>
                <Form.Select
                  id="orgSelect"
                  className="input-style shadow-sm maxInputWidth"
                  name="org"
                >
                  {Object.values(orgs.restOrgs).map((value) => {
                    return (
                      <option key={value.orgId} value={value.orgId}>
                        {value.orgName}
                      </option>
                    );
                  })}
                </Form.Select>
              </Form.Group>
              <Container className="px-0 maxInputWidth ms-0">
                <Row className="justify-content-between">
                  <Col className="align-self-start" xs="4">
                    <Form.Group className="mb-4">
                      <Form.Label className="blue-main-text">Taxes:</Form.Label>
                      <Form.Select
                        id="taxSelect"
                        className="input-style shadow-sm"
                        name="taxes"
                      >
                        {Object.values(taxes).map((value) => {
                          return (
                            <option key={value.taxesId} value={value.taxesId}>
                              {value.taxesValue}
                            </option>
                          );
                        })}
                      </Form.Select>
                    </Form.Group>
                  </Col>
                  <Col className="align-self-end" xs="4">
                    <Form.Group className="mb-4">
                      <Form.Label className="blue-main-text">
                        Currency:
                      </Form.Label>
                      <Form.Select
                        id="currencySelect"
                        className="input-style shadow-sm"
                        disabled={products.length > 0}
                        name="currency"
                        onChange={(e) => {
                          if (products.length > 0) return;
                          if (e.target.value != "PLN") {
                            setShowCurrencyExchange(true);
                            setChosenCurrency(e.target.value);
                          } else {
                            setShowCurrencyExchange(false);
                          }
                        }}
                      >
                        <option value={"PLN"}>PLN</option>
                        <option value={"EUR"}>EUR</option>
                        <option value={"USD"}>USD</option>
                      </Form.Select>
                    </Form.Group>
                  </Col>
                </Row>
              </Container>
              {showCurrencyExchange ? (
                <Form.Group className="mb-3">
                  <Form.Label className="blue-main-text">
                    Currency Exchange:
                  </Form.Label>
                  <ErrorMessage
                    message={currencyList.message}
                    messageStatus={currencyList.error}
                  />
                  <Form.Select
                    className="input-style shadow-sm maxInputWidth"
                    type="text"
                    name="currencyExchange"
                    isInvalid={currencyList.error}
                  >
                    {currencyList.error
                      ? null
                      : currencyList.rates.map((val) => {
                          return (
                            <option
                              key={val.effectiveDate}
                              value={[val.effectiveDate, val.mid]}
                            >
                              {val.mid} Date: {val.effectiveDate}
                            </option>
                          );
                        })}
                  </Form.Select>
                </Form.Group>
              ) : null}
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Transport cost:
                </Form.Label>
                <ErrorMessage
                  message="Is empty or is not a number."
                  messageStatus={transportError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="transport"
                  placeholder="transport cost"
                  isInvalid={transportError}
                  onInput={(e) => {
                    InputValidator.decimalValidator(
                      e.target.value,
                      setTransportError,
                    );
                  }}
                />
              </Form.Group>
              <Container className="px-0 maxInputWidth ms-0">
                <Row className="justify-content-between">
                  <Col className="align-self-start" xs="6" sm="4">
                    <Form.Group className="mb-4">
                      <Form.Label className="blue-main-text">
                        Payment method:
                      </Form.Label>
                      <Form.Select
                        id="methodSelect"
                        className="input-style shadow-sm"
                        name="paymentMethod"
                      >
                        {Object.values(paymentMethods).map((value) => {
                          return (
                            <option
                              key={value.paymentMethodId}
                              value={value.paymentMethodId}
                            >
                              {value.methodName}
                            </option>
                          );
                        })}
                      </Form.Select>
                    </Form.Group>
                  </Col>
                  <Col className="align-self-end" xs="6" sm="4">
                    <Form.Group className="mb-4">
                      <Form.Label className="blue-main-text">
                        Payment status:
                      </Form.Label>
                      <Form.Select
                        id="statusSelect"
                        className="input-style shadow-sm"
                        name="paymentStatus"
                      >
                        {Object.values(paymentStatuses).map((value) => {
                          return (
                            <option
                              key={value.paymentStatusId}
                              value={value.paymentStatusId}
                            >
                              {value.statusName}
                            </option>
                          );
                        })}
                      </Form.Select>
                    </Form.Group>
                  </Col>
                </Row>
              </Container>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">Status:</Form.Label>
                <Form.Select
                  className="input-style shadow-sm maxInputWidth"
                  name="status"
                >
                  <option value={false}>Not in system</option>
                  <option value={true}>In system</option>
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
                        key={[
                          value.id,
                          value.itemId,
                          value.priceId,
                          value.invoiceId,
                          key,
                        ]}
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
                    if (isYourInvoice) {
                      setShowProductWindow(true);
                    } else {
                      setShowSalesProductWindow(true);
                    }
                  }}
                >
                  Add Product
                </Button>
              </Form.Group>
              <Form.Group className="mb-4 maxInputWidth">
                <Form.Label className="blue-main-text">Sum:</Form.Label>
                <p>
                  {products.reduce(
                    (partialSum, obj) => partialSum + obj.price * obj.qty,
                    0,
                  )}
                </p>
              </Form.Group>
              <Form.Group className="mb-4 maxInputWidth">
                <Form.Label className="blue-main-text">Document:</Form.Label>
                <ErrorMessage
                  message="Must be a pdf file or not empty."
                  messageStatus={documentError}
                />
                <Form.Control
                  type="file"
                  accept=".pdf"
                  isInvalid={documentError}
                  onChange={(e) => {
                    if (e.target.value.endsWith("pdf")) {
                      setDocumentError(false);
                      let formData = new FormData();
                      formData.append("file", e.target.files[0]);
                      setFile(formData);
                    } else {
                      setDocumentError(true);
                    }
                  }}
                />
              </Form.Group>
              <Form.Group className="mb-5 pb-5" controlId="formDescription">
                <Form.Label className="blue-main-text maxInputWidth-desc">
                  Note:
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm"
                  as="textarea"
                  rows={5}
                  type="text"
                  name="description"
                  placeholder="description"
                  maxLength={500}
                />
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

                        let form = document.getElementById("invoiceForm");
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
        <AddProductWindow
          modalShow={showProductWindow}
          onHideFunction={() => setShowProductWindow(false)}
          addFunction={(val) => {
            products.push(val);
          }}
        />
        <AddSaleProductWindow
          modalShow={showSalesProductWindow}
          onHideFunction={() => setShowSalesProductWindow(false)}
          addFunction={(val) => {
            products.push(val);
          }}
          userId={chosenUser}
          currency={chosenCurrency}
          addedProductsQty={products.length}
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
            document.getElementById("invoiceForm").reset();
            setProducts([]);
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

AddInvoiceOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  isYourInvoice: PropTypes.bool.isRequired,
};

export default AddInvoiceOffcanvas;
