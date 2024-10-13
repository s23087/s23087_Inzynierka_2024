import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import getOrgsList from "@/utils/documents/get_orgs_list";
import getPaymentMethods from "@/utils/documents/get_payment_methods";
import getPaymentStatuses from "@/utils/documents/get_payment_statuses";
import CloseIcon from "../../../../../public/icons/close_black.png";
import ErrorMessage from "@/components/smaller_components/error_message";
import updateInvoice from "@/utils/documents/modify_invoice";
import StringValidtor from "@/utils/validators/form_validator/stringValidator";
import getRestModifyInvoice from "@/utils/documents/get_rest_modify_info";

function ModifyInvoiceOffcanvas({
  showOffcanvas,
  hideFunction,
  isYourInvoice,
  invoice,
}) {
  const router = useRouter();
  useEffect(() => {
    if (showOffcanvas) {
      const orgs = getOrgsList();
      orgs.then((data) => setOrgs(data));
      const paymentMethods = getPaymentMethods();
      paymentMethods.then((data) => setPaymentMethods(data));
      const paymentStatuses = getPaymentStatuses();
      paymentStatuses.then((data) => setPaymentStatuses(data));
      const restInfo = getRestModifyInvoice(invoice.invoiceId);
      restInfo.then((data) => {
        setRestInfo(data);
        prevState.invoiceNumber = invoice.invoiceNumber;
        prevState.transport = data.transport;
        prevState.paymentMethod = data.paymentMethod;
        prevState.note = data.note;
        prevState.status = invoice.inSystem;
      });
    }
  }, [showOffcanvas]);
  // rest info
  const [restInfo, setRestInfo] = useState({
    transport: "is loading",
    paymentMethod: "is loading",
    note: "is loading",
  });
  // options
  const [orgs, setOrgs] = useState({
    restOrgs: [],
  });
  const [paymentMethods, setPaymentMethods] = useState([]);
  const [paymentStatuses, setPaymentStatuses] = useState([]);
  // File
  const [file, setFile] = useState();
  // Errors
  const [invoiceNumberError, setInvoiceNumberError] = useState(false);
  const [transportError, setTransportError] = useState(false);
  const [documentError, setDocumentError] = useState(false);
  const anyErrorActive = invoiceNumberError || transportError || documentError;
  // Misc
  const [isLoading, setIsLoading] = useState(false);
  // Form
  const [prevState] = useState({
    invoiceNumber: "",
    client: -1,
    transport: 0.0,
    paymentMethod: -1,
    paymentStatus: -1,
    status: null,
    note: "",
  });
  const [state, formPurchaseAction] = useFormState(
    updateInvoice
      .bind(null, file)
      .bind(null, isYourInvoice)
      .bind(null, invoice.invoiceId)
      .bind(null, prevState),
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
                  Modify {isYourInvoice ? "purchase" : "sales"} invoice
                </p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
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
        <Offcanvas.Body className="px-4 px-xl-5 mx-1 mx-xl-3 pb-0" as="div">
          <Container className="p-0" style={vhStyle} fluid>
            <Form
              className="mx-1 mx-xl-4"
              id="invoiceForm"
              action={formPurchaseAction}
            >
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Invoice Number:
                </Form.Label>
                <ErrorMessage
                  message="Is empty, not a number or lenght is greater than 40."
                  messageStatus={invoiceNumberError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="invoice"
                  isInvalid={invoiceNumberError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(
                      e.target.value,
                      setInvoiceNumberError,
                      40,
                    );
                  }}
                  maxLength={40}
                  defaultValue={invoice.invoiceNumber}
                />
              </Form.Group>
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
                  <option value={-1}>{invoice.clientName}</option>
                  {Object.values(orgs.restOrgs)
                    .filter((e) => e.orgName !== invoice.clientName)
                    .map((value) => {
                      return (
                        <option key={value.orgId} value={value.orgId}>
                          {value.orgName}
                        </option>
                      );
                    })}
                </Form.Select>
              </Form.Group>
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
                  key={restInfo.transport}
                  name="transport"
                  defaultValue={restInfo.transport}
                  isInvalid={transportError}
                  onInput={(e) => {
                    StringValidtor.decimalValidator(
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
                        <option value={-1}>{restInfo.paymentMethod}</option>
                        {Object.values(paymentMethods)
                          .filter(
                            (e) => e.methodName !== restInfo.paymentMethod,
                          )
                          .map((value) => {
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
                        <option value={-1}>{invoice.paymentStatus}</option>
                        {Object.values(paymentStatuses)
                          .filter((e) => e.statusName !== invoice.paymentStatus)
                          .map((value) => {
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
                  {invoice.inSystem ? (
                    <>
                      <option value={true}>In system</option>
                      <option value={false}>Not in system</option>
                    </>
                  ) : (
                    <>
                      <option value={false}>Not in system</option>
                      <option value={true}>In system</option>
                    </>
                  )}
                </Form.Select>
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
              <Form.Group className="mb-5" controlId="formDescription">
                <Form.Label className="blue-main-text maxInputWidth-desc">
                  Note:
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm"
                  as="textarea"
                  rows={5}
                  key={restInfo.note}
                  type="text"
                  name="note"
                  defaultValue={restInfo.note}
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
                      disabled={anyErrorActive}
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
                        "Save"
                      )}
                    </Button>
                  </Col>
                  <Col>
                    <Button
                      variant="red"
                      className="w-100"
                      onClick={() => {
                        hideFunction();
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

ModifyInvoiceOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  isYourInvoice: PropTypes.bool.isRequired,
  invoice: PropTypes.object.isRequired,
};

export default ModifyInvoiceOffcanvas;
