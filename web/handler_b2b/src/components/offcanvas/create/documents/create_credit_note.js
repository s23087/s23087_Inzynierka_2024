import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import CloseIcon from "../../../../../public/icons/close_black.png";
import ProductHolder from "@/components/smaller_components/product_holder";
import ErrorMessage from "@/components/smaller_components/error_message";
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import getListOfPurchaseInvoice from "@/utils/documents/get_list_purchase_invoice";
import getListOfSalesInvoice from "@/utils/documents/get_list_sales_invoice";
import AddCreditProductWindow from "@/components/windows/add_credit_items";
import CreateCreditNote from "@/utils/documents/create_credit_note";
import getUsers from "@/utils/flexible/get_users";

/**
 * Create offcanvas that allow to create credit note.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {boolean} props.isYourCreditNote If type equal to "Yours credit notes" then true, otherwise false.
 * @return {JSX.Element} Offcanvas element
 */
function AddCreditNoteOffcanvas({
  showOffcanvas,
  hideFunction,
  isYourCreditNote,
}) {
  const router = useRouter();
  // download data holders
  const [users, setUsers] = useState([]);
  const [invoiceList, setInvoiceList] = useState([]);
  const [chosenInvoice, setChosenInvoice] = useState();
  const [chosenClient, setChosenClient] = useState();
  const [userOrg, setUserOrg] = useState();
  // download error
  const [userDownloadError, setUserDownloadError] = useState(false);
  const [invoiceListDownloadError, setInvoiceListDownloadError] =
    useState(false);
  // download data
  useEffect(() => {
    if (showOffcanvas) {
      getUsers().then((data) => {
        if (data !== null) {
          setUserDownloadError(false);
          setUsers(data);
        } else {
          setUserDownloadError(true);
        }
      });
      const invoiceList = isYourCreditNote
        ? getListOfPurchaseInvoice()
        : getListOfSalesInvoice();
      invoiceList.then((data) => {
        if (data === null) {
          setInvoiceListDownloadError(true);
          return;
        }
        setInvoiceListDownloadError(false);
        setInvoiceList(data);
        setChosenInvoice(data[0].invoiceId ? data[0].invoiceId : null);
        setChosenClient(data[0].clientName ?? "Is loading");
        setUserOrg(data[0].orgName ?? "Is loading");
      });
    }
  }, [showOffcanvas, isYourCreditNote]);
  // useState for showing ad product window
  const [showProductWindow, setShowProductWindow] = useState(false);
  // product holder
  const [products, setProducts] = useState([]);
  // key variable for rerendering component with products
  const [resetSeed, setResetSeed] = useState(false);
  // File holder
  const [file, setFile] = useState();
  // Form errors
  const [creditNumberError, setCreditNumberError] = useState(false);
  const [documentError, setDocumentError] = useState(false);
  const [dateError, setDateError] = useState(false);
  /**
   * Check if form can be submitted
   */
  const isFormErrorActive = () =>
    creditNumberError ||
    documentError ||
    dateError ||
    chosenClient === "Is loading" ||
    userOrg === "Is loading" ||
    products.length === 0 ||
    userDownloadError ||
    invoiceListDownloadError;
  // True if create action is running
  const [isLoading, setIsLoading] = useState(false);
  // Form action
  const [state, formPurchaseAction] = useFormState(
    CreateCreditNote.bind(null, {
      userOrg: userOrg,
      chosenClient: chosenClient,
      isYourCreditNote: isYourCreditNote,
      invoiceId: chosenInvoice,
    })
      .bind(null, products)
      .bind(null, file),
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
                <p className="blue-main-text h4 mb-0">
                  Create {isYourCreditNote ? "your" : "client"} credit note
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
              id="creditNoteForm"
              action={formPurchaseAction}
            >
              <ErrorMessage
                message="Could not download all necessary information."
                messageStatus={userDownloadError || invoiceListDownloadError}
              />
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">User:</Form.Label>
                <Form.Select
                  id="userSelect"
                  className="input-style shadow-sm maxInputWidth"
                  name="user"
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
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">
                  Invoice number:
                </Form.Label>
                <Form.Select
                  id="userSelect"
                  className="input-style shadow-sm maxInputWidth"
                  name="invoice"
                  disabled={products.length > 0}
                  onChange={(e) => {
                    let targetVal = parseInt(e.target.value);
                    setChosenInvoice(targetVal);
                    let invIndex = invoiceList.findIndex(
                      (e) => e.invoiceId === targetVal,
                    );
                    if (invIndex !== -1) {
                      setChosenClient(invoiceList[invIndex].clientName);
                    }
                  }}
                >
                  {invoiceList.map((val) => {
                    return (
                      <option key={val.invoiceId} value={val.invoiceId}>
                        {val.invoiceNumber}
                      </option>
                    );
                  })}
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Credit note number:
                </Form.Label>
                <ErrorMessage
                  message="Is empty, not a number or length is greater than 40."
                  messageStatus={creditNumberError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="creditNumber"
                  placeholder="credit note number"
                  isInvalid={creditNumberError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setCreditNumberError,
                      40,
                    );
                  }}
                  maxLength={40}
                />
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">Date:</Form.Label>
                <ErrorMessage
                  message="Date exceed today's date."
                  messageStatus={dateError}
                />
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
                  }}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  {isYourCreditNote ? "For:" : "Source:"}
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="orgName"
                  key={userOrg}
                  defaultValue={userOrg ?? "Is loading"}
                  readOnly
                  disabled
                />
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">
                  {isYourCreditNote ? "Source:" : "For:"}
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="clientName"
                  key={chosenClient}
                  defaultValue={chosenClient ?? "Is loading"}
                  readOnly
                  disabled
                />
              </Form.Group>
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
              <Form.Group className="mb-4">
                <Form.Check
                  className="blue-sec-text"
                  type="checkbox"
                  label="Is paid?"
                  name="isPaid"
                />
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
                  disabled={!chosenInvoice}
                  onClick={() => {
                    setShowProductWindow(true);
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

                        let form = document.getElementById("creditNoteForm");
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
        <AddCreditProductWindow
          modalShow={showProductWindow}
          onHideFunction={() => setShowProductWindow(false)}
          addFunction={(val) => {
            products.push(val);
          }}
          addedProducts={products}
          invoiceId={chosenInvoice}
          isYourCredit={isYourCreditNote}
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
            document.getElementById("creditNoteForm").reset();
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

AddCreditNoteOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  isYourCreditNote: PropTypes.bool.isRequired,
};

export default AddCreditNoteOffcanvas;
