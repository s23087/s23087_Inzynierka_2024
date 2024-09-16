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
import StringValidtor from "@/utils/validators/form_validator/stringValidator";

function ModifyClientOffcanvas({ showOffcanvas, hideFunction, client, isOrg }) {
  const router = useRouter();
  // View change
  const [isBindingView, setIsBindingView] = useState(false);
  // Object data
  const [restInfo, setRestInfo] = useState({});
  const [countries, setCountries] = useState([]);
  const [statues, setStatuses] = useState([]);
  const [clientBindings, setClientBindings] = useState([]);
  const [users, setUsers] = useState([]);
  // Get data & set prev state
  const [prevState, setPrevState] = useState({});
  useEffect(() => {
    if (showOffcanvas) {
      const restData = getRestClientInfo(client.clientId);
      restData.then((data) => setRestInfo(data));
      const countries = getCountries();
      countries.then((data) => setCountries(data));
      const statuses = getAvailabilityStatuses();
      statuses.then((data) => setStatuses(data));
      setPrevState({
        orgName: client.clientName,
        street: client.street,
        city: client.city,
        postalCode: client.postal,
        statusId:
          Object.values(statues).findIndex(
            (e) => e.name === restInfo.availability,
          ) + 1,
      });
    }
  }, [showOffcanvas]);
  useEffect(() => {
    if (isBindingView) {
      const bindings = getUserClientBindings(client.clientId);
      bindings.then((data) => setClientBindings(data));
      const users = getUsers();
      users.then((data) => setUsers(data));
    }
  }, [isBindingView]);
  // Errors
  const [nameError, setNameError] = useState(false);
  const [nipError, setNipError] = useState(false);
  const [streetError, setStreetError] = useState(false);
  const [cityError, setCityError] = useState(false);
  const [postalError, setPostalError] = useState(false);
  const [creditError, setCreditError] = useState(false);
  const resetErrors = () => {
    setNameError(false)
    setNipError(false)
    setStreetError(false)
    setCityError(false)
    setPostalError(false)
    setCreditError(false)
  }
  const getIsErrorActive = () => {
    return nameError || nipError || streetError || cityError || postalError || creditError;
  }
  const anyErrorActive = getIsErrorActive();
  // Loading
  const [isLoading, setIsLoading] = useState(false);
  const [isBindingLoading, setIsBindingLoading] = useState(false);
  // Bindings states
  const [bindingChanged, setBindingChanged] = useState(false);
  const [bindingSuccess, setBindingSuccess] = useState(false);
  const [bindingFailure, setBindingFailure] = useState(false);
  const [bindingMessage, setBindingMessage] = useState(false);
  // Form
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
  const vhStyleHidden = {
    height: "81vh",
    display: "none",
  };
  const vhStyleUnhidden = {
    height: "81vh",
    display: "block",
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
                  className="ps-0"
                >
                  <Image
                    src={
                      isBindingView ? switch_binding_view : switch_product_view
                    }
                    style={isOrg ? unhidden : hidden}
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
                    if (!state.error && state.completed) {
                      router.refresh();
                    }
                    if (bindingSuccess) {
                      router.refresh();
                    }
                    setIsBindingView(false);
                    setBindingMessage("");
                    setBindingFailure(false);
                    setBindingSuccess(false);
                    resetErrors();
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
          <Container
            className="p-0"
            style={isBindingView ? vhStyleHidden : vhStyleUnhidden}
            fluid
          >
            <Form
              className="mx-1 mx-xl-4"
              id="clientModify"
              action={formAction}
            >
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Name:</Form.Label>
                <ErrorMessage message="Is empty or lenght is greater than 50." messageStatus={nameError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="name"
                  defaultValue={client.clientName}
                  isInvalid={nameError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(e.target.value, setNameError, 50)
                  }}
                  maxLength={50}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Nip:</Form.Label>
                <ErrorMessage message="Is empty, not a number or lenght is greater than 15." messageStatus={nipError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="nip"
                  defaultValue={client.nip}
                  isInvalid={nipError}
                  onInput={(e) => {
                    StringValidtor.emptyNumberStringValidtor(e.target.value, setNipError, 15)
                  }}
                  maxLength={15}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Street:</Form.Label>
                <ErrorMessage message="Is empty or lenght is greater than 200." messageStatus={streetError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="street"
                  defaultValue={client.street}
                  isInvalid={streetError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(e.target.value, setStreetError, 200)
                  }}
                  maxLength={200}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">City:</Form.Label>
                <ErrorMessage message="Is empty or lenght is greater than 200." messageStatus={cityError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="city"
                  defaultValue={client.city}
                  isInvalid={cityError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(e.target.value, setCityError, 200)
                  }}
                  maxLength={200}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Postal code:</Form.Label>
                <ErrorMessage message="Is empty or lenght is greater than 25." messageStatus={postalError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="postal"
                  defaultValue={client.postal}
                  isInvalid={postalError}
                  onInput={(e) => {
                    StringValidtor.onlyNumberStringValidtor(e.target.value, setPostalError, 25)
                  }}
                  maxLength={25}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Credit Limit:
                </Form.Label>
                <ErrorMessage message="Is empty or lenght is greater than 25." messageStatus={creditError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="credit"
                  defaultValue={restInfo.creditLimit}
                  isInvalid={creditError}
                  onInput={(e) => {
                    StringValidtor.emptyNumberStringValidtor(e.target.value, setCreditError, 25)
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
                  <option
                    key={
                      Object.values(statues).findIndex(
                        (e) => e.name === restInfo.availability,
                      ) + 1
                    }
                    value={
                      Object.values(statues).findIndex(
                        (e) => e.name === restInfo.availability,
                      ) + 1
                    }
                  >
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
                <Row style={maxStyle} className="mx-auto minScalableWidth">
                  <Col>
                    <Button
                      variant="mainBlue"
                      className="w-100"
                      type="click"
                      disabled={anyErrorActive}
                      onClick={(e) => {
                        e.preventDefault();
                        if (anyErrorActive) return;
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
            className="p-0 pt-3"
            style={isBindingView ? vhStyleUnhidden : vhStyleHidden}
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

  function resetState() {
    state.error = false;
    state.completed = false;
    state.message = "";
    setIsLoading(false);
  }
}

ModifyClientOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  client: PropTypes.object.isRequired,
  isOrg: PropTypes.bool.isRequired,
};

export default ModifyClientOffcanvas;
