import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import CloseIcon from "../../../../../public/icons/close_black.png";
import ErrorMessage from "@/components/smaller_components/error_message";
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import getRestModifyCredit from "@/utils/documents/get_rest_modify_credit";
import updateCreditNote from "@/utils/documents/modify_credit_note";

/**
 * Create offcanvas that allow to modify chosen credit note.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {boolean} props.isYourCredit If type equal to "Yours credit notes" then true, otherwise false.
 * @param {{proformaId: Number, proformaNumber: string, clientName: string, transport: Number}} props.creditNote Chosen credit note to view.
 * @return {JSX.Element} Offcanvas element
 */
function ModifyCreditNoteOffcanvas({
  showOffcanvas,
  hideFunction,
  isYourCredit,
  creditNote,
}) {
  const router = useRouter();
  // Download data holder
  const [restInfo, setRestInfo] = useState({
    creditNumber: "Is loading",
    orgName: "Is loading",
    note: "Is loading",
  });
  // Credit note previous data holder
  const [prevState] = useState({
    creditNoteNumber: "",
    date: "",
    inSystem: false,
    isPaid: false,
    note: "",
  });
  // Download error
  const [restDownloadError, setRestDownloadError] = useState(false);
  // Download data
  useEffect(() => {
    if (showOffcanvas) {
      getRestModifyCredit(creditNote.creditNoteId, isYourCredit).then(
        (data) => {
          if (data === null) {
            setRestDownloadError(true);
          } else {
            setRestDownloadError(false);
            setRestInfo(data);
            prevState.creditNoteNumber = data.creditNumber;
            prevState.note = data.note;
          }
        },
      );
    }
  }, [showOffcanvas]);
  // File holder
  const [file, setFile] = useState();
  // From errors
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
    restInfo.creditNumber === "Is loading" ||
    restDownloadError;
  // True if modify action is running
  const [isLoading, setIsLoading] = useState(false);
  // Form action
  const [state, formPurchaseAction] = useFormState(
    updateCreditNote
      .bind(null, file)
      .bind(null, isYourCredit)
      .bind(null, creditNote.creditNoteId)
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
                  Modify {isYourCredit ? "your" : "client"} credit note
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
        <Offcanvas.Body className="px-4 px-xl-5 pb-0" as="div">
          <Container className="p-0" style={vhStyle} fluid>
            <Form
              className="mx-1 mx-xl-3"
              id="creditModifyForm"
              action={formPurchaseAction}
            >
              <ErrorMessage
                message="Error: could not download all necessary info."
                messageStatus={restDownloadError}
              />
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Credit Note Number:
                </Form.Label>
                <ErrorMessage
                  message="Is empty, not a number or length is greater than 40."
                  messageStatus={creditNumberError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="creditNumber"
                  isInvalid={creditNumberError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setCreditNumberError,
                      40,
                    );
                  }}
                  maxLength={40}
                  key={restInfo.creditNumber}
                  defaultValue={restInfo.creditNumber}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">For Invoice:</Form.Label>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  defaultValue={creditNote.invoiceNumber}
                  readOnly
                  disabled
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Date:</Form.Label>
                <ErrorMessage
                  message="Date exceed today's date."
                  messageStatus={dateError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  name="date"
                  type="date"
                  defaultValue={new Date(creditNote.date).toLocaleDateString(
                    "en-CA",
                  )}
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
                  {isYourCredit ? "For:" : "Source:"}
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  key={restInfo.orgName}
                  defaultValue={restInfo.orgName}
                  readOnly
                  disabled
                />
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">
                  {isYourCredit ? "Source:" : "For:"}
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  defaultValue={creditNote.clientName}
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
                  {creditNote.inSystem ? (
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
              <Form.Group className="mb-4">
                <Form.Check
                  className="blue-sec-text"
                  type="checkbox"
                  label="Is paid?"
                  name="isPaid"
                  defaultChecked={creditNote.isPaid}
                />
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
                  type="text"
                  name="note"
                  key={restInfo.note}
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
                      disabled={isFormErrorActive()}
                      onClick={(e) => {
                        e.preventDefault();
                        setIsLoading(true);

                        prevState.inSystem = creditNote.inSystem;
                        prevState.isPaid = creditNote.isPaid;
                        prevState.date = creditNote.date.substring(0, 10);

                        let form = document.getElementById("creditModifyForm");
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
            document.getElementById("creditModifyForm").reset();
            hideFunction();
            router.refresh();
          }}
        />
      </Container>
    </Offcanvas>
  );

  /**
   * Reset state of form
  */
  function resetState() {
    state.error = false;
    state.completed = false;
    state.message = "";
    setIsLoading(false);
  }
}

ModifyCreditNoteOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  isYourCredit: PropTypes.bool.isRequired,
  creditNote: PropTypes.object.isRequired,
};

export default ModifyCreditNoteOffcanvas;
