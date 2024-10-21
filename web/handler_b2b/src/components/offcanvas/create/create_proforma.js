import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import validators from "@/utils/validators/validator";
import getUsers from "@/utils/flexible/get_users";
import getOrgsList from "@/utils/documents/get_orgs_list";
import getTaxes from "@/utils/documents/get_taxes";
import getPaymentMethods from "@/utils/documents/get_payment_methods";
import CloseIcon from "../../../../public/icons/close_black.png";
import ProductHolder from "@/components/smaller_components/product_holder";
import AddProductWindow from "@/components/windows/addProduct";
import getCurrencyValuesList from "@/utils/flexible/get_currency_values_list";
import AddSaleProductWindow from "@/components/windows/add_Sales_product";
import ErrorMessage from "@/components/smaller_components/error_message";
import InputValidtor from "@/utils/validators/form_validator/inputValidator";
import createProforma from "@/utils/proformas/create_proforma";

function AddProformaOffcanvas({ showOffcanvas, hideFunction, isYourProforma }) {
  const router = useRouter();
  // download errors
  const [userDownloadError, setUserDownloadError] = useState(false);
  const [orgDownloadError, setOrgDownloadError] = useState(false);
  const [taxesDownloadError, setTaxesDownloadError] = useState(false);
  const [methodsDownloadError, setMethodsDownloadError] = useState(false);
  // options
  const [users, setUsers] = useState([]);
  const [orgs, setOrgs] = useState({
    restOrgs: [],
  });
  const [taxes, setTaxes] = useState([]);
  const [paymentMethods, setPaymentMethods] = useState([]);
  useEffect(() => {
    if (showOffcanvas) {
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

      getUsers().then((data) => {
        if (data === null) {
          setUserDownloadError(true);
        } else {
          setUserDownloadError(false);
          setUsers(data);
          setChoosenUser(data[0].idUser);
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
    }
  }, [showOffcanvas]);
  // products
  const [showProductWindow, setShowProductWindow] = useState(false);
  const [showSalesProductWindow, setShowSalesProductWindow] = useState(false);
  const [products, setProducts] = useState([]);
  const [resetSeed, setResetSeed] = useState(false);
  // File
  const [file, setFile] = useState();
  // Chossen user
  const [choosenUser, setChoosenUser] = useState();
  // Curenncy exchange value
  const [showCurrencyExchange, setShowCurrencyExchange] = useState(false);
  const [choosenCurrency, setChoosenCurrency] = useState("PLN");
  const [invoiceDate, setInvoiceDate] = useState(
    new Date().toLocaleDateString("en-CA"),
  );
  const [currencyList, setCurrencyList] = useState({
    error: false,
    message: "",
    rates: [],
  });
  useEffect(() => {
    if (showCurrencyExchange) {
      if (Date.parse(invoiceDate) > Date.now()) {
        setCurrencyList({
          error: true,
          message: "There is no exisitng value for this proforma date.",
          rates: [],
        });
        return;
      }
      let list = getCurrencyValuesList(
        choosenCurrency,
        invoiceDate,
        new Date().toLocaleDateString("en-CA"),
      );
      list
        .then((data) =>
          setCurrencyList({
            error: data.error,
            rates: data.rates,
          }),
        )
        .catch(() => {
          setCurrencyList({
            error: true,
            message: "Critical error",
            rates: [],
          });
        });
    }
  }, [showCurrencyExchange, invoiceDate, choosenCurrency]);
  // Errors
  const [proformaError, setProformaError] = useState(false);
  const [transportError, setTransportError] = useState(false);
  const [documentError, setDocumentError] = useState(false);
  const [dateError, setDateError] = useState(false);
  const isFormErrorActive = () =>
    proformaError ||
    transportError ||
    documentError ||
    currencyList.error ||
    dateError ||
    orgDownloadError ||
    userDownloadError ||
    taxesDownloadError ||
    methodsDownloadError ||
    orgs.restOrgs.length === 0 ||
    products.length === 0;
  // Misc
  const [isLoading, setIsLoading] = useState(false);
  // Form
  const [state, createProformaAction] = useFormState(
    createProforma
      .bind(null, products)
      .bind(null, file)
      .bind(null, orgs)
      .bind(null, isYourProforma),
    {
      error: false,
      completed: false,
      message: "",
    },
  );
  // Styles
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };
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
                <p className="blue-main-text h4 mb-0">
                  Create {isYourProforma ? "your" : "client"} proforma
                </p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                    setProducts([]);
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
            <Form
              className="mx-1 mx-xl-3"
              id="proformaForm"
              action={createProformaAction}
            >
              <ErrorMessage
                message="Error: could not download all necessary info."
                messageStatus={
                  orgDownloadError ||
                  userDownloadError ||
                  taxesDownloadError ||
                  methodsDownloadError
                }
              />
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">User:</Form.Label>
                <Form.Select
                  id="userSelect"
                  className="input-style shadow-sm maxInputWidth"
                  name="user"
                  disabled={products.length > 0}
                  onChange={(e) => {
                    setChoosenUser(e.target.value);
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
                  Proforma Number:
                </Form.Label>
                <ErrorMessage
                  message="Is empty, not a number or lenght is greater than 40."
                  messageStatus={proformaError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="proformaNumber"
                  placeholder="proforma number"
                  isInvalid={proformaError}
                  onInput={(e) => {
                    InputValidtor.normalStringValidtor(
                      e.target.value,
                      setProformaError,
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
                    Warning! The choosen day is not a week day, therfore
                    currency values will be taken from last friday.
                  </p>
                </Row>
                <Row className="justify-content-between">
                  <Col>
                    <Form.Group className="mb-4">
                      <Form.Label className="blue-main-text">Date:</Form.Label>
                      <Form.Control
                        className="input-style shadow-sm maxInputWidth"
                        type="date"
                        name="date"
                        isInvalid={dateError}
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
                        message="Date excceed today's date."
                        messageStatus={dateError}
                      />
                    </Form.Group>
                  </Col>
                </Row>
              </Container>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  {isYourProforma ? "For:" : "Source:"}
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
                  {isYourProforma ? "Source:" : "For:"}
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
                        name="currency"
                        onChange={(e) => {
                          if (e.target.value != "PLN") {
                            setShowCurrencyExchange(true);
                            setChoosenCurrency(e.target.value);
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
                    Currency Exchange
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
                            <option key={val} value={val.effectiveDate}>
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
                    if (
                      validators.isPriceFormat(e.target.value) &&
                      validators.stringIsNotEmpty(e.target.value)
                    ) {
                      setTransportError(false);
                    } else {
                      setTransportError(true);
                    }
                  }}
                />
              </Form.Group>
              <Container className="px-0 maxInputWidth ms-0">
                <Row>
                  <Col>
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
                  {products.map((value) => {
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
                    if (isYourProforma) {
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

                        let form = document.getElementById("proformaForm");
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
          userId={choosenUser}
          currency={choosenCurrency}
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
            document.getElementById("proformaForm").reset();
            setProducts([]);
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

AddProformaOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  isYourProforma: PropTypes.bool.isRequired,
};

export default AddProformaOffcanvas;
