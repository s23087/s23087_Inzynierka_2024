import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import getOrgsList from "@/utils/documents/get_orgs_list";
import getPaymentMethods from "@/utils/documents/get_payment_methods";
import CloseIcon from "../../../../public/icons/close_black.png";
import ErrorMessage from "@/components/smaller_components/error_message";
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import getRestModifyProforma from "@/utils/proformas/get_rest_modify_proforma";
import getUsers from "@/utils/flexible/get_users";
import updateProforma from "@/utils/proformas/modify_proforma";

/**
 * Create offcanvas that allow to modify chosen proforma.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {boolean} props.isYourProforma If type equal to "Yours proformas" then true, otherwise false.
 * @param {{proformaId: Number, proformaNumber: string, clientName: string, transport: Number}} props.proforma Chosen proforma to view.
 * @return {JSX.Element} Offcanvas element
 */
function ModifyProformaOffcanvas({
  showOffcanvas,
  hideFunction,
  isYourProforma,
  proforma,
}) {
  const router = useRouter();
  // download data holders
  const [restInfo, setRestInfo] = useState({
    userId: -1,
    inSystem: false,
    paymentMethod: "Is loading",
    note: "Is loading",
  });
  const [users, setUsers] = useState([]);
  const [orgs, setOrgs] = useState({
    orgName: "Is loading",
    restOrgs: [],
  });
  const [paymentMethods, setPaymentMethods] = useState([]);
  // download error
  const [userDownloadError, setUserDownloadError] = useState(false);
  const [orgDownloadError, setOrgDownloadError] = useState(false);
  const [methodsDownloadError, setMethodsDownloadError] = useState(false);
  const [restDownloadError, setRestDownloadError] = useState(false);
  // download data
  useEffect(() => {
    if (showOffcanvas) {
      getUsers().then((data) => {
        if (data === null) {
          setUserDownloadError(true);
        } else {
          setUserDownloadError(false);
          setUsers(data);
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

      getPaymentMethods().then((data) => {
        if (data !== null) {
          setMethodsDownloadError(false);
          setPaymentMethods(data);
        } else {
          setMethodsDownloadError(true);
        }
      });

      getRestModifyProforma(proforma.proformaId).then((data) => {
        if (data === null) {
          setRestDownloadError(true);
        } else {
          setRestDownloadError(false);
          setRestInfo(data);
          prevState.proformaNumber = proforma.proformaNumber;
          prevState.transport = proforma.transport;
          prevState.note = data.note;
          prevState.status = data.inSystem;
          prevState.userId = data.userId;
        }
      });
    }
  }, [showOffcanvas]);
  // For file input
  const [file, setFile] = useState();
  // Form errors
  const [proformaNumberError, setInvoiceNumberError] = useState(false);
  const [transportError, setTransportError] = useState(false);
  const [documentError, setDocumentError] = useState(false);
  /**
   * Check if form can be submitted
   */
  const isFormErrorActive = () =>
    proformaNumberError ||
    transportError ||
    documentError ||
    orgs.orgName === "Is loading" ||
    restInfo.note === "Is loading" ||
    userDownloadError ||
    restDownloadError ||
    methodsDownloadError ||
    orgDownloadError;
  // True if modify action is running
  const [isLoading, setIsLoading] = useState(false);
  // Previous state of proforma. -1 if number property do not changed
  const [prevState] = useState({
    proformaNumber: "",
    userId: -1,
    client: -1,
    transport: 0.0,
    paymentMethod: -1,
    status: null,
    note: "",
  });
  // Form action with state
  const [state, formPurchaseAction] = useFormState(
    updateProforma
      .bind(null, file)
      .bind(null, orgs)
      .bind(null, prevState)
      .bind(null, proforma.proformaId)
      .bind(null, isYourProforma),
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
                  Modify {isYourProforma ? "your" : "client"} proforma
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
        <Offcanvas.Body className="px-4 px-xl-5 pb-0 scrollableHeight" as="div">
          <Container className="p-0" fluid>
            <Form
              className="mx-1 mx-xl-3"
              id="proformaInvoice"
              action={formPurchaseAction}
            >
              <ErrorMessage
                message="Error: could not download all necessary info."
                messageStatus={
                  userDownloadError ||
                  restDownloadError ||
                  methodsDownloadError ||
                  orgDownloadError
                }
              />
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">User:</Form.Label>
                <Form.Select
                  id="userSelect"
                  className="input-style shadow-sm maxInputWidth"
                  name="user"
                  key={restInfo.userId}
                  defaultValue={restInfo.userId}
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
                  message="Is empty, not a number or length is greater than 40."
                  messageStatus={proformaNumberError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="proformaNumber"
                  isInvalid={proformaNumberError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setInvoiceNumberError,
                      40,
                    );
                  }}
                  maxLength={40}
                  defaultValue={proforma.proformaNumber}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  {isYourProforma ? "For:" : "Source:"}
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  key={orgs.orgName}
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
                  <option value={-1}>{proforma.clientName}</option>
                  {Object.values(orgs.restOrgs)
                    .filter((e) => e.orgName !== proforma.clientName)
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
                  key={proforma.transport}
                  name="transport"
                  defaultValue={proforma.transport}
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
                </Row>
              </Container>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">Status: </Form.Label>
                <Form.Select
                  className="input-style shadow-sm maxInputWidth"
                  name="status"
                  key={restInfo.inSystem}
                  defaultValue={restInfo.inSystem}
                >
                  <option value={false}>Not in system</option>
                  <option value={true}>In system</option>
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-4 maxInputWidth">
                <Form.Label className="blue-main-text">Document:</Form.Label>
                <ErrorMessage
                  message="Must be a pdf file."
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
                  key={restInfo.note}
                  type="text"
                  name="note"
                  defaultValue={restInfo.note}
                  maxLength={500}
                />
              </Form.Group>
              <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
                <Row style={maxStyle} className="mx-auto minScalableWidth offcanvasButtonsStyle">
                  <Col>
                    <Button
                      variant="mainBlue"
                      className="w-100"
                      type="submit"
                      disabled={isFormErrorActive()}
                      onClick={(e) => {
                        e.preventDefault();
                        setIsLoading(true);

                        let form = document.getElementById("proformaInvoice");
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
            document.getElementById("proformaInvoice").reset();
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

ModifyProformaOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  isYourProforma: PropTypes.bool.isRequired,
  proforma: PropTypes.object.isRequired,
};

export default ModifyProformaOffcanvas;
