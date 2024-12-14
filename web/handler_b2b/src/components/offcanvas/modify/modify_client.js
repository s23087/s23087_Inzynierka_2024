import Image from "next/image";
import PropTypes from "prop-types";
import {
  Offcanvas,
  Container,
  Row,
  Col,
  Button,
  Form,
  InputGroup,
  Stack,
} from "react-bootstrap";
import CloseIcon from "../../../../public/icons/close_black.png";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import modifyClient from "@/utils/flexible/modify_client";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import ErrorMessage from "@/components/smaller_components/error_message";
import getRestClientInfo from "@/utils/clients/get_rest_info";
import getCountries from "@/utils/flexible/get_countries";
import getAvailabilityStatuses from "@/utils/clients/get_availability_statuses";
import switch_product_view from "../../../../public/icons/switch_product_view.png";
import switch_binding_view from "../../../../public/icons/switch_binding_view.png";
import getUserClientBindings from "@/utils/clients/get_client_bindings";
import getUsers from "@/utils/flexible/get_users";
import setUserClientBindings from "@/utils/clients/set_user_clients_bindings";
import InputValidator from "@/utils/validators/form_validator/inputValidator";

/**
 * Create offcanvas that allow to modify chosen client.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {{clientId: Number, clientName: string, street: string, city: string, postal: string, nip: Number|undefined, country: string, countryId: Number}} props.client Chosen client to view.
 * @param {boolean} props.isOrg True if org view is activated.
 * @return {JSX.Element} Offcanvas element
 */
function ModifyClientOffcanvas({ showOffcanvas, hideFunction, client, isOrg }) {
  const router = useRouter();
  // View change. If true binding modification is shown and client modify is hidden
  const [isBindingView, setIsBindingView] = useState(false);
  // Download holders
  const [restInfo, setRestInfo] = useState({
    creditLimit: null,
    availability: "",
    daysForRealization: null,
  });
  const [countries, setCountries] = useState([]);
  const [statues, setStatuses] = useState([]);
  const [clientBindings, setClientBindings] = useState([]);
  const [users, setUsers] = useState([]);
  // Previous data holder
  const [prevState, setPrevState] = useState({});
  // download error
  const [statusesDownloadError, setStatusesDownloadError] = useState(false);
  const [bindingsDownloadError, setBindingsDownloadError] = useState(false);
  const [restDownloadError, setRestDownloadError] = useState(false);
  const [countriesDownloadError, setCountriesDownloadError] = useState(false);
  const [userDownloadError, setUserDownloadError] = useState(false);
  // download data
  useEffect(() => {
    if (showOffcanvas) {
      getRestClientInfo(client.clientId).then((data) => {
        if (data !== null) {
          setRestInfo(data);
          setRestDownloadError(false);
        } else {
          setRestDownloadError(true);
        }
      });
      getCountries().then((data) => {
        if (data !== null) {
          setCountriesDownloadError(false);
          setCountries(data);
        } else {
          setCountriesDownloadError(true);
        }
      });
      getAvailabilityStatuses().then((data) => {
        if (data !== null) {
          setStatuses(data);
          setStatusesDownloadError(false);
        } else {
          setStatusesDownloadError(true);
        }
      });
      setPrevState({
        orgName: client.clientName,
        street: client.street,
        city: client.city,
        postalCode: client.postal,
        statusId: -1,
      });
    }
  }, [showOffcanvas]);
  useEffect(() => {
    if (isBindingView) {
      getUserClientBindings(client.clientId).then((data) => {
        if (data !== null) {
          setClientBindings(data);
          setBindingsDownloadError(false);
        } else {
          setBindingsDownloadError(true);
        }
      });
      getUsers().then((data) => {
        if (data === null) {
          setUserDownloadError(true);
        } else {
          setUserDownloadError(false);
          setUsers(data);
        }
      });
    }
  }, [isBindingView]);
  // Form errors
  const [nameError, setNameError] = useState(false);
  const [nipError, setNipError] = useState(false);
  const [streetError, setStreetError] = useState(false);
  const [cityError, setCityError] = useState(false);
  const [postalError, setPostalError] = useState(false);
  const [creditError, setCreditError] = useState(false);
  /**
   * Reset form errors
   */
  const resetErrors = () => {
    setNameError(false);
    setNipError(false);
    setStreetError(false);
    setCityError(false);
    setPostalError(false);
    setCreditError(false);
  };
  /**
   * Check if form can be submitted
   */
  const getIsErrorActive = () => {
    return (
      nameError ||
      nipError ||
      streetError ||
      cityError ||
      postalError ||
      creditError ||
      statusesDownloadError ||
      bindingsDownloadError ||
      restDownloadError ||
      countriesDownloadError ||
      userDownloadError
    );
  };
  // True if modify action is running
  const [isLoading, setIsLoading] = useState(false);
  // True if binding modification is running
  const [isBindingLoading, setIsBindingLoading] = useState(false);
  // Bindings rerender key
  const [bindingChanged, setBindingChanged] = useState(false);
  // True if modify binding operation is success
  const [bindingSuccess, setBindingSuccess] = useState(false);
  // True if modify binding operation is failure
  const [bindingFailure, setBindingFailure] = useState(false);
  // Message from modify binding result
  const [bindingMessage, setBindingMessage] = useState(false);
  // Form action
  const [state, formAction] = useFormState(
    modifyClient.bind(null, client.clientId).bind(null, prevState),
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
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };
  const buttonStyle = {
    maxWidth: "250px",
    borderRadius: "5px",
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
              <Col xs="6" lg="9" xl="10" className="d-flex align-items-center">
                <p className="blue-main-text h4 mb-0">Modify Org</p>
              </Col>
              <Col xs="4" lg="2" xl="1" className="ps-1 text-end">
                <Button
                  variant="as-link"
                  onClick={() => {
                    setIsBindingView(!isBindingView);
                  }}
                  style={isOrg ? unhidden : hidden}
                  className="ps-0"
                >
                  <Image
                    src={
                      isBindingView ? switch_binding_view : switch_product_view
                    }
                    className="h-auto w-auto"
                    alt="switch view"
                  />
                </Button>
              </Col>
              <Col xs="2" lg="1" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                    setIsBindingView(false);
                    setBindingMessage("");
                    setBindingFailure(false);
                    setBindingSuccess(false);
                    resetErrors();
                    setIsLoading(false);
                    state.completed = false;
                    if (!state.error && state.completed) {
                      router.refresh();
                    }
                    if (bindingSuccess) {
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
          <Container
            className="p-0 scrollableHeight"
            style={isBindingView ? hidden : unhidden}
            fluid
          >
            <ErrorMessage
              message="Error: could not download all necessary info."
              messageStatus={
                statusesDownloadError ||
                bindingsDownloadError ||
                restDownloadError ||
                countriesDownloadError ||
                userDownloadError
              }
            />
            <Form
              className="mx-1 mx-xl-3 pb-5"
              id="clientModify"
              action={formAction}
            >
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Name:</Form.Label>
                <ErrorMessage
                  message="Is empty or length is greater than 50."
                  messageStatus={nameError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="name"
                  defaultValue={client.clientName}
                  isInvalid={nameError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setNameError,
                      50,
                    );
                  }}
                  maxLength={50}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Nip:</Form.Label>
                <ErrorMessage
                  message="Is empty, not a number or length is greater than 9."
                  messageStatus={nipError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="nip"
                  defaultValue={client.nip}
                  isInvalid={nipError}
                  onInput={(e) => {
                    InputValidator.emptyNumberStringValidator(
                      e.target.value,
                      setNipError,
                      9,
                    );
                  }}
                  maxLength={15}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Street:</Form.Label>
                <ErrorMessage
                  message="Is empty or length is greater than 200."
                  messageStatus={streetError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="street"
                  defaultValue={client.street}
                  isInvalid={streetError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setStreetError,
                      200,
                    );
                  }}
                  maxLength={200}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">City:</Form.Label>
                <ErrorMessage
                  message="Is empty or length is greater than 200."
                  messageStatus={cityError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="city"
                  defaultValue={client.city}
                  isInvalid={cityError}
                  onInput={(e) => {
                    InputValidator.normalStringValidator(
                      e.target.value,
                      setCityError,
                      200,
                    );
                  }}
                  maxLength={200}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Postal code:</Form.Label>
                <ErrorMessage
                  message="Is empty or length is greater than 25."
                  messageStatus={postalError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="postal"
                  defaultValue={client.postal}
                  isInvalid={postalError}
                  onInput={(e) => {
                    InputValidator.onlyNumberStringValidator(
                      e.target.value,
                      setPostalError,
                      25,
                    );
                  }}
                  maxLength={25}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Credit Limit:
                </Form.Label>
                <ErrorMessage
                  message="Is empty or length is greater than 25."
                  messageStatus={creditError}
                />
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="credit"
                  defaultValue={restInfo.creditLimit}
                  isInvalid={creditError}
                  onInput={(e) => {
                    InputValidator.emptyNumberStringValidator(
                      e.target.value,
                      setCreditError,
                      25,
                    );
                  }}
                  maxLength={25}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Country:</Form.Label>
                <Form.Select
                  id="countrySelect"
                  className="input-style shadow-sm maxInputWidth"
                  name="country"
                >
                  <option
                    key={
                      Object.values(countries).findIndex(
                        (e) => e.countryName === client.country,
                      ) + 1
                    }
                    value={
                      Object.values(countries).findIndex(
                        (e) => e.countryName === client.country,
                      ) + 1
                    }
                  >
                    {client.country}
                  </option>
                  {Object.values(countries)
                    .filter((e) => e.id !== client.countryId)
                    .map((value) => {
                      return (
                        <option key={value.id} value={value.id}>
                          {value.countryName}
                        </option>
                      );
                    })}
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-5 pb-4">
                <Form.Label className="blue-main-text">
                  Availability:{" "}
                </Form.Label>
                <Form.Select
                  className="input-style shadow-sm maxInputWidth"
                  name="availability"
                >
                  <option value={-1} key={restInfo.availability}>
                    {restInfo.availability}
                  </option>
                  {Object.values(statues)
                    .filter((e) => e.name !== restInfo.availability)
                    .map((value) => {
                      return (
                        <option key={value.id} value={value.id}>
                          {value.name + " (" + value.days + ")"}
                        </option>
                      );
                    })}
                </Form.Select>
              </Form.Group>
              <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
                <Row style={maxStyle} className="mx-auto minScalableWidth offcanvasButtonsStyle">
                  <Col>
                    <Button
                      variant="mainBlue"
                      className="w-100"
                      type="click"
                      disabled={getIsErrorActive()}
                      onClick={(e) => {
                        e.preventDefault();
                        if (getIsErrorActive()) return;
                        setIsLoading(true);
                        let form = document.getElementById("clientModify");
                        form.requestSubmit();
                      }}
                    >
                      {isLoading && !state.completed ? (
                        <div className="spinner-border main-text"></div>
                      ) : (
                        "Save Changes"
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
                        if (bindingSuccess) {
                          router.refresh();
                        }
                        setIsBindingView(false);
                        resetErrors();
                      }}
                    >
                      Cancel
                    </Button>
                  </Col>
                </Row>
              </Container>
            </Form>
          </Container>
          <Container
            className="p-0 pt-3 px-1 px-xl-3 scrollableHeight"
            style={isBindingView ? unhidden : hidden}
            fluid
          >
            <p
              className="text-start mb-1 green-main-text"
              style={bindingSuccess ? unhidden : hidden}
            >
              {bindingMessage}
            </p>
            <p
              className="text-start mb-1 red-sec-text"
              style={bindingFailure ? unhidden : hidden}
            >
              {bindingMessage}
            </p>
            <InputGroup className="mb-3">
              <Form.Select
                id="userSelect"
                className="input-style shadow-sm"
                name="user"
                key={users}
              >
                {Object.values(users)
                  .filter(
                    (e) =>
                      Object.values(clientBindings).findIndex(
                        (d) => d.idUser === e.idUser,
                      ) === -1,
                  )
                  .map((value) => {
                    return (
                      <option key={value.idUser} value={value.idUser}>
                        {value.username + " " + value.surname}
                      </option>
                    );
                  })}
              </Form.Select>
              <Button
                variant="mainBlue"
                onClick={() => {
                  setBindingFailure(false);
                  setBindingSuccess(false);
                  let inputVal = document.getElementById("userSelect").value;
                  if (inputVal == "") return;
                  setBindingMessage("");
                  clientBindings.push(
                    Object.values(users).filter(
                      (e) => e.idUser === parseInt(inputVal),
                    )[0],
                  );
                  setBindingChanged(!bindingChanged);
                }}
              >
                Add
              </Button>
            </InputGroup>
            <ul key={[bindingChanged, clientBindings]}>
              {Object.values(users)
                .filter(
                  (e) =>
                    Object.values(clientBindings).findIndex(
                      (d) => d.idUser === e.idUser,
                    ) !== -1,
                )
                .map((val) => (
                  <li key={val.idUser} className="mb-2">
                    <Stack direction="horizontal">
                      <span>{val.username + " " + val.surname}</span>
                      <Button
                        variant="red"
                        className="ms-auto"
                        onClick={() => {
                          setBindingMessage("");
                          setBindingFailure(false);
                          setBindingSuccess(false);
                          setClientBindings(
                            clientBindings.filter(
                              (e) => e.idUser !== val.idUser,
                            ),
                          );
                        }}
                      >
                        Delete
                      </Button>
                    </Stack>
                  </li>
                ))}
            </ul>
            <Button
              variant="mainBlue"
              className="mb-3 mt-2 ms-3 py-3 w-100"
              style={buttonStyle}
              onClick={async () => {
                let ids = [];
                Object.values(clientBindings).forEach((e) =>
                  ids.push(e.idUser),
                );
                setIsBindingLoading(true);
                let isOk = await setUserClientBindings(client.clientId, ids);
                setIsBindingLoading(false);
                if (isOk.ok) {
                  setBindingSuccess(true);
                  setBindingMessage(isOk.message);
                } else {
                  setBindingFailure(true);
                  setBindingMessage(isOk.message);
                }
              }}
            >
              {isBindingLoading ? (
                <div className="spinner-border main-text"></div>
              ) : (
                "Save Changes"
              )}
            </Button>
          </Container>
        </Offcanvas.Body>

        <Toastes.ErrorToast
          showToast={state.completed && state.error}
          message={state.message}
          onHideFun={() => {
            resetState();
            router.refresh();
          }}
        />
        <Toastes.SuccessToast
          showToast={state.completed && !state.error}
          message={state.message}
          onHideFun={() => {
            resetState();
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

ModifyClientOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  client: PropTypes.object.isRequired,
  isOrg: PropTypes.bool.isRequired,
};

export default ModifyClientOffcanvas;
