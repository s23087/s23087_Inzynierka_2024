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
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import updateItem from "@/utils/warehouse/update_item";
import SpecialInput from "@/components/smaller_components/special_input";
import BindingInput from "@/components/smaller_components/binding_input";
import AddEanWindow from "@/components/windows/addEan";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import getDescription from "@/utils/warehouse/get_description";
import getBindings from "@/utils/warehouse/get_bindings";
import CloseIcon from "../../../../public/icons/close_black.png";
import switch_product_view from "../../../../public/icons/switch_product_view.png";
import switch_binding_view from "../../../../public/icons/switch_binding_view.png";
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import ErrorMessage from "@/components/smaller_components/error_message";
import ChangeBindingsWindow from "@/components/windows/change_bindings_window";
import changeBindings from "@/utils/warehouse/change_bindings";
import validators from "@/utils/validators/validator";

/**
 * Create offcanvas that allow to modify chosen item.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {{itemId: Number, eans: Array<string>, partNumber: string, itemName: string}} props.item Chosen item to view.
 * @param {string} props.currency Shortcut name of the currency that user choose.
 * @param {boolean} props.isOrg True if org view is activated.
 * @return {JSX.Element} Offcanvas element
 */
function ModifyItemOffcanvas({
  showOffcanvas,
  hideFunction,
  item,
  currency,
  isOrg,
}) {
  const router = useRouter();
  // Previous state of item holder
  const [prevState] = useState({
    itemId: null,
    name: null,
    description: null,
    partNumber: null,
  });
  // switch from product modify to binding modify
  const [isProductView, setIsProductView] = useState(true);
  // Eans
  const [eans, setEans] = useState([]);
  // useState for change binding window
  const [showBindingWindow, setShowBindingWindow] = useState(false);
  // Holder for chosen binding to modify passed to window element
  const [bindingToModify, setBindingToModify] = useState({
    invoiceNumber: "",
    qty: 0,
  });
  // New and changed bindings holder
  const [bindingsChanges] = useState([]);
  // Form errors
  const [partnumberError, setPartnumberError] = useState(false);
  const [nameError, setNameError] = useState(false);
  // Download data holders
  const [bindings, setBindings] = useState([
    {
      username: "user name",
      qty: "qty",
      price: "price",
      currency: "PLN",
      invoiceNumber: "invoice number",
    },
  ]);
  const [description, setDescription] = useState(null);
  // Download error
  const [descriptionDownloadError, setDescriptionDownloadError] =
    useState(false);
  const [bindingsDownloadError, setBindingsDownloadError] = useState(false);
  // Download data
  useEffect(() => {
    if (showOffcanvas && isOrg) {
      let copyArray = [...item.eans];
      setEans(copyArray);
      getDescription(item.itemId).then((data) => {
        if (data !== null) {
          setDescriptionDownloadError(false);
          setDescription(data);
        } else {
          setDescriptionDownloadError(true);
        }
      });
      getBindings(item.itemId, currency).then((data) => {
        if (data !== null) {
          setBindingsDownloadError(false);
          setBindings(data);
        } else {
          setBindingsDownloadError(true);
        }
      });
    } else if (showOffcanvas) {
      let copyArray = [...item.eans];
      setEans(copyArray);
      getDescription(item.itemId).then((data) => {
        if (data !== null) {
          setDescriptionDownloadError(false);
          setDescription(data);
        } else {
          setDescriptionDownloadError(true);
        }
      });
    }
  }, [showOffcanvas]);
  // Rerender Eans
  const [seedChange, setRerenderVar] = useState(1);
  /**
   * Check if ean exist in eans array
   * @param {string} variable New ean
   */
  const eanExist = (variable) => {
    return eans.findIndex((item) => item === variable) != -1;
  };
  // useState for add ean window
  const [isAddEanShow, setIsAddEanShow] = useState(false);
  // True if modify action is running
  const [isLoading, setIsLoading] = useState(false);
  /**
   * Check if form can be submitted
   */
  const isFormErrorActive = () =>
    partnumberError ||
    nameError ||
    descriptionDownloadError ||
    bindingsDownloadError;
  /**
   * Reset form error
   */
  const resetErrors = () => {
    setPartnumberError(false);
    setNameError(false);
  };
  // Forms action
  const [state, formAction] = useFormState(
    updateItem.bind(null, eans).bind(null, prevState),
    {
      error: false,
      completed: false,
      message: "",
    },
  );
  const [bindingState, setBindingState] = useState({
    error: false,
    completed: false,
    message: "",
  });
  // Styles
  const maxStyle = {
    maxWidth: "393px",
  };
  const buttonStyle = {
    maxWidth: "250px",
    borderRadius: "5px",
  };
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };
  return (
    <Offcanvas
      className="h-100 minScalableWidth"
      show={showOffcanvas}
      onHide={hideFunction}
      placement="bottom"
    >
      <Container
        className="h-100 w-100 p-0"
        onMouseMove={() => {
          if (state.completed) {
            setIsLoading(false);
          }
        }}
        fluid
      >
        <Offcanvas.Header className="border-bottom-grey px-xl-5">
          <Container className="px-3" fluid>
            <Row>
              <Col xs="6" lg="9" xl="10" className="d-flex align-items-center">
                <p className="blue-main-text h4 mb-0">Id: {item.itemId}</p>
              </Col>
              <Col xs="4" lg="2" xl="1" className="ps-1 text-end">
              {isOrg ? 
                <Button
                  variant="as-link"
                  onClick={() => {
                    setIsProductView(!isProductView);
                  }}
                  className="ps-0"
                >
                  <Image
                    src={
                      isProductView ? switch_product_view : switch_binding_view
                    }
                    className="h-auto w-auto"
                    alt="switch view"
                  />
                </Button> : null}
              </Col>
              <Col xs="2" lg="1" className="ps-1 text-end">
                <Button
                  variant="as-link"
                  onClick={() => {
                    resetErrors();
                    setIsProductView(true)
                    hideFunction();
                  }}
                  className="ps-2 pe-0"
                >
                  <Image src={CloseIcon} alt="Hide" />
                </Button>
              </Col>
            </Row>
          </Container>
        </Offcanvas.Header>
        <Offcanvas.Body className="px-4 px-xl-5 mx-1 mx-xl-3 pb-0 scrollableHeight" as="div">
          <Container
            className="p-0"
            style={isProductView ? unhidden : hidden}
            fluid
          >
            <Form action={formAction} id="modifyItemForm">
              <ErrorMessage
                message="Could not download all necessary information."
                messageStatus={
                  descriptionDownloadError || bindingsDownloadError
                }
              />
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
                  isInvalid={partnumberError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setPartnumberError,
                      150,
                    );
                  }}
                  defaultValue={item.partNumber}
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
                  isInvalid={nameError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setNameError,
                      250,
                    );
                  }}
                  defaultValue={item.itemName}
                  maxLength={250}
                />
              </Form.Group>
              <Form.Group className="mb-3" key={eans}>
                <Stack key={seedChange}>
                  <Form.Label className="blue-main-text">EANs:</Form.Label>
                  {eans.map((val, key) => {
                    return (
                      <SpecialInput
                        value={val}
                        key={val}
                        deleteValue={() => {
                          eans.splice(key, 1);
                          if (seedChange === 1) {
                            setRerenderVar(seedChange + 1);
                          }
                          if (seedChange > 1) {
                            setRerenderVar(seedChange - 1);
                          }
                        }}
                        modifyValue={(variable) => (eans[key] = variable)}
                        existFun={eanExist}
                        validatorFunc={(val) => validators.haveOnlyNumbers(val)}
                        errorMessage="Ean already exist or have letters"
                      />
                    );
                  })}
                  <Button
                    variant="mainBlue"
                    className="mb-3 mt-4 ms-3 py-3"
                    style={buttonStyle}
                    onClick={() => setIsAddEanShow(true)}
                  >
                    Add Ean
                  </Button>
                  <AddEanWindow
                    modalShow={isAddEanShow}
                    onHideFunction={() => setIsAddEanShow(false)}
                    addAction={(variable) => eans.push(variable)}
                    eanExistFun={eanExist}
                  />
                </Stack>
              </Form.Group>
              <Form.Group className="mb-5 pb-5" key={description}>
                <Form.Label className="blue-main-text maxInputWidth-desc">
                  Description:
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm"
                  as="textarea"
                  rows={5}
                  key={description}
                  type="text"
                  name="description"
                  defaultValue={description !== undefined ? description : "Loading.."}
                  maxLength={500}
                />
              </Form.Group>
            </Form>
          </Container>
          <Container
            className="p-0 scrollableHeight"
            style={!isProductView ? unhidden : hidden}
            fluid
          >
            <Form className="pb-5">
              <Form.Group className="mb-3" controlId="formDescription">
                <Form.Label className="blue-main-text maxInputWidth">
                  Users:
                </Form.Label>
                {bindings
                  .filter((e) => e.qty > 0)
                  .map((value, key) => {
                    return (
                      <BindingInput
                        value={value}
                        key={[
                          key,
                          value.username,
                          value.userId,
                          value.qty,
                          value.price,
                          value.currency,
                          value.invoiceNumber,
                          value.invoiceId,
                        ]}
                        modifyAction={() => {
                          setBindingToModify(value);
                          setShowBindingWindow(true);
                        }}
                      />
                    );
                  })}
                <ChangeBindingsWindow
                  modalShow={showBindingWindow}
                  onHideFunction={() => setShowBindingWindow(false)}
                  value={bindingToModify}
                  addBinding={(oldVal, newUserId, qty, username) => {
                    bindingsChanges.push({
                      userId: newUserId,
                      invoiceId: oldVal.invoiceId,
                      itemId: item.itemId,
                      qty: qty,
                    });
                    bindingsChanges.push({
                      userId: oldVal.userId,
                      invoiceId: oldVal.invoiceId,
                      itemId: item.itemId,
                      qty: qty * -1,
                    });
                    bindings[
                      bindings.findIndex(
                        (e) =>
                          e.userId === oldVal.userId &&
                          e.invoiceId === oldVal.invoiceId,
                      )
                    ].qty -= qty;
                    if (
                      bindings.findIndex(
                        (e) =>
                          e.userId === newUserId &&
                          e.invoiceId === oldVal.invoiceId,
                      ) !== -1
                    ) {
                      bindings[
                        bindings.findIndex(
                          (e) =>
                            e.userId === newUserId &&
                            e.invoiceId === oldVal.invoiceId,
                        )
                      ].qty += qty;
                    } else {
                      bindings.push({
                        userId: newUserId,
                        username: username,
                        qty: qty,
                        price: oldVal.price,
                        currency: oldVal.currency,
                        invoiceNumber: oldVal.invoiceNumber,
                        invoiceId: oldVal.invoiceId,
                      });
                    }
                  }}
                />
              </Form.Group>
              <Button
                variant="mainBlue"
                className="mb-3 mt-2 ms-3 py-3 w-100"
                style={buttonStyle}
                disabled={bindingsChanges.length === 0}
                type="click"
                onClick={async (e) => {
                  e.preventDefault();
                  if (bindingsChanges.length === 0) {
                    return;
                  }
                  let result = await changeBindings(bindingsChanges);
                  setBindingState(result);
                }}
              >
                Save
              </Button>
            </Form>
          </Container>
          <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
            <Row style={maxStyle} className="mx-auto minScalableWidth offcanvasButtonsStyle">
              <Col>
                <Button
                  variant="mainBlue"
                  className="w-100"
                  disabled={isFormErrorActive()}
                  onClick={(e) => {
                    e.preventDefault();
                    if (isFormErrorActive()) return;
                    setIsLoading(true);
                    prevState.itemId = item.itemId;
                    prevState.name = item.itemName;
                    prevState.description = description;
                    prevState.partNumber = item.partNumber;
                    let form = document.getElementById("modifyItemForm");
                    form.requestSubmit();
                  }}
                >
                  {isLoading && !state.complete ? (
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
                    resetErrors();
                    setIsProductView(true)
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
        </Offcanvas.Body>
        <Container key={state.completed}>
          <Toastes.ErrorToast
            showToast={
              (state.completed && state.error) ||
              (bindingState.completed && bindingState.error)
            }
            message={state.completed ? state.message : bindingState.message}
            key={[
              state.completed,
              state.error,
              bindingState.completed,
              bindingState.error,
            ]}
            onHideFun={() => {
              resetState();
            }}
          />
          <Toastes.SuccessToast
            showToast={
              (state.completed && !state.error) ||
              (bindingState.completed && !bindingState.error)
            }
            message={state.completed ? state.message : bindingState.message}
            key={[
              state.completed,
              !state.error,
              bindingState.completed,
              !bindingState.error,
            ]}
            onHideFun={() => {
              resetState();
              hideFunction();
              router.refresh();
            }}
          />
        </Container>
      </Container>
    </Offcanvas>
  );

  function resetState() {
    state.error = false;
    state.completed = false;
    state.message = "";
    bindingState.error = false;
    bindingState.completed = false;
    bindingState.message = "";
    setIsLoading(false);
  }
}

ModifyItemOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  item: PropTypes.object.isRequired,
  currency: PropTypes.string.isRequired,
  isOrg: PropTypes.bool.isRequired,
};

export default ModifyItemOffcanvas;
