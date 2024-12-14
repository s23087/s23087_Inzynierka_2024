import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import CloseIcon from "../../../../../public/icons/close_black.png";
import ErrorMessage from "@/components/smaller_components/error_message";
import getReceivers from "@/utils/documents/get_request_users";
import createRequest from "@/utils/documents/create_request";
import InputValidator from "@/utils/validators/form_validator/inputValidator";

/**
 * Create offcanvas that allow to create request.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @return {JSX.Element} Offcanvas element
 */
function AddRequestOffcanvas({ showOffcanvas, hideFunction }) {
  const router = useRouter();
  // download data holder
  const [users, setUsers] = useState([]);
  // download error
  const [userDownloadError, setUserDownloadError] = useState(false);
  // download data
  useEffect(() => {
    if (showOffcanvas) {
      getReceivers().then((data) => {
        if (data !== null) {
          setUserDownloadError(false);
          setUsers(data);
        } else {
          setUserDownloadError(true);
        }
      });
    }
  }, [showOffcanvas]);
  // File holder
  const [file, setFile] = useState();
  // Form errors
  const [documentError, setDocumentError] = useState(false);
  const [noteError, setNoteError] = useState(false);
  const [titleError, setTitleError] = useState(false);
  /**
   * Check if form can be submitted
   */
  const isFromErrorActive = () =>
    documentError ||
    noteError ||
    users.length === 0 ||
    titleError ||
    userDownloadError;
  // True if create action is running
  const [isLoading, setIsLoading] = useState(false);
  // Form action
  const [state, formAction] = useFormState(createRequest.bind(null, file), {
    error: false,
    completed: false,
    message: "",
  });
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
                <p className="blue-main-text h4 mb-0">Create request</p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                    resetState();
                    document.getElementById("requestForm").reset();
                    if (!state.error && state.complete) {
                      router.refresh();
                    }
                    setNoteError(false);
                    setDocumentError(false);
                    setTitleError(false);
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
            <Form className="mx-1 mx-xl-3" id="requestForm" action={formAction}>
              <ErrorMessage
                message="Error: could not download all necessary info."
                messageStatus={userDownloadError}
              />
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">Title:</Form.Label>
                <ErrorMessage
                  message="Is empty or length is greater than 100."
                  messageStatus={titleError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="title"
                  placeholder="title"
                  id="title"
                  isInvalid={titleError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setTitleError,
                      100,
                    );
                  }}
                  maxLength={100}
                />
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">Receiver:</Form.Label>
                <Form.Select
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
                <Form.Label className="blue-main-text">Object type:</Form.Label>
                <Form.Select
                  className="input-style shadow-sm maxInputWidth"
                  name="type"
                >
                  <option value="Sales invoices">Sales invoices</option>
                  <option value="Yours invoices">Yours invoices</option>
                  <option value="Yours credit notes">Yours credit notes</option>
                  <option value="Client credit notes">
                    Client credit notes
                  </option>
                  <option value="Yours Proformas">Yours Proformas</option>
                  <option value="Clients Proformas">Clients Proformas</option>
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
              <Form.Group className="mb-5 pb-5">
                <Form.Label className="blue-main-text maxInputWidth-desc">
                  Note:
                </Form.Label>
                <ErrorMessage
                  message="Note can not be empty."
                  messageStatus={noteError}
                />
                <Form.Control
                  className="input-style shadow-sm"
                  as="textarea"
                  rows={5}
                  type="text"
                  name="note"
                  id="noteId"
                  isInvalid={noteError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setNoteError,
                      500,
                    );
                  }}
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
                      disabled={isFromErrorActive()}
                      onClick={(e) => {
                        e.preventDefault();
                        let note = document.getElementById("noteId").value;
                        let title = document.getElementById("title").value;
                        if (!note || !title) {
                          if (!note) setNoteError(true);
                          if (!title) setTitleError(true);
                          return;
                        }
                        setIsLoading(true);

                        let form = document.getElementById("requestForm");
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
                        document.getElementById("requestForm").reset();
                        if (!state.error && state.completed) {
                          router.refresh();
                          resetState();
                        } else {
                          resetState();
                        }
                        setNoteError(false);
                        setDocumentError(false);
                        setTitleError(false);
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
            setNoteError(false);
            setDocumentError(false);
            setTitleError(false);
            document.getElementById("requestForm").reset();
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

AddRequestOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
};

export default AddRequestOffcanvas;
