import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import CloseIcon from "../../../../../public/icons/close_black.png";
import ErrorMessage from "@/components/smaller_components/error_message";
import getReceviers from "@/utils/documents/get_request_users";
import InputValidtor from "@/utils/validators/form_validator/inputValidator";
import getRestModifyRequest from "@/utils/documents/get_rest_modify_request";
import updateRequest from "@/utils/documents/modify_request";

function ModifyRequestOffcanvas({ showOffcanvas, hideFunction, request }) {
  const router = useRouter();
  const [userDownloadError, setUserDownloadError] = useState(false);
  const [restDownloadError, setRestDownloadError] = useState(false);
  useEffect(() => {
    if (showOffcanvas) {
      getReceviers().then((data) => {
        if (data !== null) {
          setUserDownloadError(false);
          setUsers(data);
        } else {
          setUserDownloadError(true);
        }
      });

      getRestModifyRequest(request.id).then((data) => {
        if (data === null) {
          setRestDownloadError(true);
          return;
        }
        setRestDownloadError(false);
        setRestInfo(data);
        prevState.recevierId = data.recevierId;
        prevState.note = data.note;
      });
      prevState.objectType = request.objectType;
      prevState.title = request.title;
    }
  }, [showOffcanvas]);
  // rest info
  const [restInfo, setRestInfo] = useState({
    recevierId: 0,
    note: "is loading",
  });
  const [prevState] = useState({
    recevierId: 0,
    objectType: "",
    note: "",
    title: "",
  });
  // options
  const [users, setUsers] = useState([]);
  // File
  const [file, setFile] = useState();
  // Errors
  const [documentError, setDocumentError] = useState(false);
  const [noteError, setNoteError] = useState(false);
  const [titleError, setTitleError] = useState(false);
  const isFormErrorActive = () =>
    documentError ||
    noteError ||
    titleError ||
    users.length === 0 ||
    request.status !== "In progress" ||
    restInfo.recevierId === 0 ||
    userDownloadError ||
    restDownloadError;
  // Misc
  const [isLoading, setIsLoading] = useState(false);
  // Form
  const [state, formAction] = useFormState(
    updateRequest.bind(null, file).bind(null, prevState).bind(null, request.id),
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
                <p className="blue-main-text h4 mb-0">Modify request</p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                    resetState();
                    document.getElementById("requestModifyForm").reset();
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
              id="requestModifyForm"
              action={formAction}
            >
              <ErrorMessage
                message="Error: could not download all necessary info."
                messageStatus={userDownloadError || restDownloadError}
              />
              <ErrorMessage
                message="You can't modify request if the status is diffrent then 'In progress.'"
                messageStatus={request.status !== "In progress"}
              />
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">Title:</Form.Label>
                <ErrorMessage
                  message="Is empty or lenght is greater than 100."
                  messageStatus={titleError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="title"
                  defaultValue={request.title}
                  id="title"
                  isInvalid={titleError}
                  onInput={(e) => {
                    InputValidtor.normalStringValidtor(
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
                  key={restInfo.recevierId}
                  defaultValue={restInfo.recevierId}
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
                  defaultValue={request.objectType}
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
              <Form.Group className="mb-5">
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
                  key={restInfo.note}
                  defaultValue={restInfo.note}
                  isInvalid={noteError}
                  onInput={(e) => {
                    InputValidtor.normalStringValidtor(
                      e.target.value,
                      setNoteError,
                      500,
                    );
                  }}
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
                        let note = document.getElementById("noteId").value;
                        if (!note) {
                          setNoteError(true);
                          return;
                        }
                        setIsLoading(true);

                        let form = document.getElementById("requestModifyForm");
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
                        document.getElementById("requestModifyForm").reset();
                        if (!state.error && state.completed) {
                          router.refresh();
                          resetState();
                        } else {
                          resetState();
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
            document.getElementById("requestModifyForm").reset();
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
    setNoteError(false);
    setDocumentError(false);
    setIsLoading(false);
  }
}

ModifyRequestOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  request: PropTypes.object.isRequired,
};

export default ModifyRequestOffcanvas;
