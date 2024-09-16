import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import CloseIcon from "../../../../public/icons/close_black.png";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import createClient from "@/utils/clients/create_client";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import getCountries from "@/utils/flexible/get_countries";
import getAvailabilityStatuses from "@/utils/clients/get_availability_statuses";
import AddAvailabilityStatusWindow from "@/components/windows/addStatus";
import StringValidtor from "@/utils/validators/form_validator/stringValidator";
import ErrorMessage from "@/components/smaller_components/error_message";

function AddClientOffcanvas({ showOffcanvas, hideFunction }) {
  const router = useRouter();
  // Get country and statuses
  useEffect(() => {
    if (showOffcanvas) {
      const countries = getCountries();
      countries.then((data) => setCountries(data));
      const statuses = getAvailabilityStatuses();
      statuses.then((data) => setStatuses(data));
    }
  }, [showOffcanvas]);
  const [countries, setCountries] = useState([]);
  const [statues, setStatuses] = useState([]);
  // Error
  const [clientNameError, setClientNameError] = useState(false);
  const [nipClientError, setClientNipError] = useState(false);
  const [clientStreetError, setClientStreetError] = useState(false);
  const [cityClientError, setClientCityError] = useState(false);
  const [clientPostalError, setClientPostalError] = useState(false);
  const [creditError, setCreditError] = useState(false);
  const setErrorsFalse = () => {
    setClientNameError(false)
    setClientNipError(false)
    setClientStreetError(false)
    setClientCityError(false)
    setClientPostalError(false)
    setCreditError(false)
  }
  const getIsErrorActive = () =>{
    return clientNameError || nipClientError || clientStreetError || cityClientError || clientPostalError || creditError;
  }
  const anyErrorActive = getIsErrorActive();
  // Add status
  const [showAddStatus, setShowAddStatus] = useState(false);
  // Loading element
  const [isLoading, setIsLoading] = useState(false);
  // Form action
  const [state, formAction] = useFormState(createClient, {
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
                <p className="blue-main-text h4 mb-0">Create client</p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                    setErrorsFalse()
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
              id="clientModify"
              action={formAction}
            >
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Name:</Form.Label>
                <ErrorMessage message="Is empty or lenght is greater than 50." messageStatus={clientNameError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="name"
                  placeholder="name"
                  isInvalid={clientNameError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(e.target.value, setClientNameError, 50)
                  }}
                  maxLength={50}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Nip:</Form.Label>
                <ErrorMessage message="Not a number or lenght is greater than 15." messageStatus={nipClientError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="nip"
                  placeholder="nip"
                  isInvalid={nipClientError}
                  onInput={(e) => {
                    StringValidtor.emptyNumberStringValidtor(e.target.value, setClientNipError, 15)
                  }}
                  maxLength={15}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Street:</Form.Label>
                <ErrorMessage message="Is empty or lenght is greater than 200." messageStatus={clientStreetError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="street"
                  placeholder="street"
                  isInvalid={clientStreetError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(e.target.value, setClientStreetError, 200)
                  }}
                  maxLength={200}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">City:</Form.Label>
                <ErrorMessage message="Is empty or lenght is greater than 200." messageStatus={cityClientError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="city"
                  placeholder="city"
                  isInvalid={cityClientError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(e.target.value, setClientCityError, 200)
                  }}
                  maxLength={200}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Postal code:</Form.Label>
                <ErrorMessage message="Is empty or lenght is greater than 25." messageStatus={clientPostalError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="postal"
                  placeholder="postal code"
                  isInvalid={clientPostalError}
                  onInput={(e) => {
                    StringValidtor.normalStringValidtor(e.target.value, setClientPostalError, 25)
                  }}
                  maxLength={25}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Credit Limit:
                </Form.Label>
                <ErrorMessage message="Must be a number or lenght is greater than 25." messageStatus={creditError}/>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="credit"
                  placeholder="credit limit"
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
                  {Object.values(countries).map((value) => {
                    return (
                      <option key={value.id} value={value.id}>
                        {value.countryName}
                      </option>
                    );
                  })}
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">
                  Availability:
                </Form.Label>
                <Form.Select
                  className="input-style shadow-sm maxInputWidth"
                  name="availability"
                >
                  {Object.values(statues).map((value) => {
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
                      type="submit"
                      disabled={anyErrorActive}
                      onClick={(e) => {
                        e.preventDefault();
                        setIsLoading(true);

                        let form = document.getElementById("clientModify");
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
                        setErrorsFalse()
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
            <Button
              variant="mainBlue"
              className="mb-5 mt-2 mx-1 mx-xl-3"
              onClick={() => setShowAddStatus(true)}
            >
              Add availability status
            </Button>
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
            router.refresh();
          }}
        />
      </Container>
      <AddAvailabilityStatusWindow
        modalShow={showAddStatus}
        onHideFunction={() => setShowAddStatus(false)}
        statuses={statues}
      />
    </Offcanvas>
  );

  function resetState() {
    state.error = false;
    state.completed = false;
    state.message = "";
    setIsLoading(false);
  }
}

AddClientOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
};

export default AddClientOffcanvas;
