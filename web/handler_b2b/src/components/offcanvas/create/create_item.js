import Image from "next/image";
import PropTypes from "prop-types";
import {
  Offcanvas,
  Container,
  Row,
  Col,
  Button,
  Form,
  Stack,
} from "react-bootstrap";
import CloseIcon from "../../../../public/icons/close_black.png";
import { useState } from "react";
import { useFormState } from "react-dom";
import createItem from "@/utils/warehouse/create_item";
import SpecialInput from "@/components/smaller_components/special_input";
import AddEanWindow from "@/components/windows/addEan";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import ErrorMessage from "@/components/smaller_components/error_message";
import validators from "@/utils/validators/validator";

/**
 * Create offcanvas that allow to create item.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @return {JSX.Element} Offcanvas element
 */
function AddItemOffcanvas({ showOffcanvas, hideFunction }) {
  const router = useRouter();
  // eans holder
  const [eans, setEans] = useState([]);
  // key variable to rerender the component
  const [rerenderVar, setRerenderVar] = useState(1);
  /**
   * Checks if ean already exist in ean array
   * @param {string} variable Ean value
   */
  const eanExist = (variable) => {
    return eans.findIndex((item) => item === variable) != -1;
  };
  // useState for showing add ean window
  const [isAddEanShow, setIsAddEanShow] = useState(false);
  // Form errors
  const [partnumberError, setPartnumberError] = useState(false);
  const [nameError, setNameError] = useState(false);
  /**
   * Check if form can be submitted
   */
  const isErrorActive = () => partnumberError || nameError;
  /**
   * Reset form errors
   */
  const resetErrors = () => {
    setPartnumberError(false);
    setNameError(false);
  };
  // Styles
  const maxStyle = {
    maxWidth: "393px",
  };
  const buttonStyle = {
    maxWidth: "250px",
    borderRadius: "5px",
  };
  // True if create action is running
  const [isLoading, setIsLoading] = useState(false);
  // Form action
  const [state, formAction] = useFormState(createItem.bind(null, eans), {
    error: false,
    completed: false,
  });
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
                <p className="blue-main-text h4 mb-0">Create item</p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    setEans([]);
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
          <Container className="p-0" fluid>
            <Form action={formAction} id="addItemForm" className="mx-1 mx-xl-3">
              <Form.Group className="mb-3 maxInputWidth">
                <Form.Label className="blue-main-text">P/N:</Form.Label>
                <ErrorMessage
                  message="Cannot be empty or exceed 150 chars."
                  messageStatus={partnumberError}
                />
                <Form.Control
                  className="input-style shadow-sm"
                  type="text"
                  name="partNumber"
                  id="partnumber"
                  isInvalid={partnumberError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setPartnumberError,
                      150,
                    );
                  }}
                  placeholder="part number"
                  maxLength={150}
                />
              </Form.Group>
              <Form.Group className="mb-3 maxInputWidth">
                <Form.Label className="blue-main-text">Name:</Form.Label>
                <ErrorMessage
                  message="Cannot be empty or exceed 250 chars."
                  messageStatus={nameError}
                />
                <Form.Control
                  className="input-style shadow-sm"
                  type="text"
                  name="name"
                  id="name"
                  isInvalid={nameError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setNameError,
                      250,
                    );
                  }}
                  placeholder="name"
                  maxLength={250}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Stack key={rerenderVar}>
                  <Form.Label className="blue-main-text">EANs:</Form.Label>
                  {eans.map((value, key) => {
                    return (
                      <SpecialInput
                        value={value}
                        key={value}
                        deleteValue={() => {
                          eans.splice(key, 1);
                          if (rerenderVar === 1) {
                            setRerenderVar(rerenderVar + 1);
                          }
                          if (rerenderVar > 1) {
                            setRerenderVar(rerenderVar - 1);
                          }
                        }}
                        existFun={eanExist}
                        modifyValue={(variable) => (eans[key] = variable)}
                        validatorFunc={(val) => validators.haveOnlyNumbers(val)}
                        errorMessage="Ean already exist or have letters"
                      />
                    );
                  })}
                  <AddEanWindow
                    modalShow={isAddEanShow}
                    onHideFunction={() => setIsAddEanShow(false)}
                    addAction={(variable) => eans.push(variable)}
                    eanExistFun={eanExist}
                  />
                  <Button
                    variant="mainBlue"
                    style={buttonStyle}
                    className="mb-3 mt-4 ms-3 py-3"
                    onClick={() => setIsAddEanShow(true)}
                  >
                    Add Ean
                  </Button>
                </Stack>
              </Form.Group>
              <Form.Group className="mb-5 pb-5" controlId="formDescription">
                <Form.Label className="blue-main-text maxInputWidth-desc">
                  Description:
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
                      disabled={isErrorActive()}
                      onClick={(e) => {
                        e.preventDefault();
                        let partnumber =
                          document.getElementById("partnumber").value;
                        let name = document.getElementById("name").value;
                        InputValidator.normalStringValidator(
                          partnumber,
                          setPartnumberError,
                          150,
                        );
                        InputValidator.normalStringValidator(
                          name,
                          setNameError,
                          250,
                        );
                        if (partnumber === "" || name === "") return;
                        if (isErrorActive()) return;
                        setIsLoading(true);
                        let addForm = document.getElementById("addItemForm");
                        addForm.requestSubmit();
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
                        resetErrors();
                        setEans([]);
                        hideFunction();
                        if (!state.error && state.completed) {
                          router.refresh();
                        }
                        resetState()
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
            hideFunction();
            setEans([]);
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

AddItemOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
};

export default AddItemOffcanvas;
