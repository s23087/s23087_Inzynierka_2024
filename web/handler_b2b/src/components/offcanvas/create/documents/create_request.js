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
import createRequest from "@/utils/documents/create_request";
import StringValidtor from "@/utils/validators/form_validator/stringValidator";

function AddRequestOffcanvas({ showOffcanvas, hideFunction }) {
  const router = useRouter();
  useEffect(() => {
    if (showOffcanvas) {
      const getUsers = getReceviers();
      getUsers.then((data) => setUsers(data));
    }
  }, [showOffcanvas]);
  // options
  const [users, setUsers] = useState([]);
  // File
  const [file, setFile] = useState();
  // Errors
  const [documentError, setDocumentError] = useState(false);
  const [noteError, setNoteError] = useState(false);
  const [titleError, setTitleError] = useState(false);
  let anyErrorActive =
    documentError || noteError || users.length === 0 || titleError;
  // Misc
  const [isLoading, setIsLoading] = useState(false);
  // Form
  const [state, formAction] = useFormState(createRequest.bind(null, file), {
    error: false,
    completed: false,
    message: "",
  });
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
            <Form className="mx-1 mx-xl-4" id="requestForm" action={formAction}>
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
                  placeholder="title"
                  id="title"
                  isInvalid={titleError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(
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
                  isInvalid={noteError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(
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
                      disabled={anyErrorActive}
                      onClick={(e) => {
                        e.preventDefault();
                        let note = document.getElementById("noteId").value;
                        let title = document.getElementById("title").value;
                        if (!note || !title) {
                          !note ? setNoteError(true) : null;
                          !title ? setTitleError(true) : null;
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
            document.getElementById("requestForm").reset();
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
    setTitleError(false);
    setIsLoading(false);
  }
}

AddRequestOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  isYourInvoice: PropTypes.bool.isRequired,
};

export default AddRequestOffcanvas;
