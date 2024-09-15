import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import CloseIcon from "../../../../public/icons/close_black.png";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import createClient from "@/utils/clients/create_client";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import validators from "@/utils/validators/validator";
import getCountries from "@/utils/flexible/get_countries";
import getAvailabilityStatuses from "@/utils/clients/get_availability_statuses";
import AddAvailabilityStatusWindow from "@/components/windows/addStatus";

function AddClientOffcanvas({ showOffcanvas, hideFunction }) {
  const router = useRouter();
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
  const [nameError, setNameError] = useState(false);
  const [nipError, setNipError] = useState(false);
  const [streetError, setStreetError] = useState(false);
  const [cityError, setCityError] = useState(false);
  const [postalError, setPostalError] = useState(false);
  const anyErrorActive =
    nameError || nipError || streetError || cityError || postalError;
  const [showAddStatus, setShowAddStatus] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [state, formAction] = useFormState(createClient, {
    error: false,
    completed: false,
    message: "",
  });
  const maxStyle = {
    maxWidth: "393px",
  };
  const vhStyle = {
    height: "81vh",
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
            <Form
              className="mx-1 mx-xl-4"
              id="clientModify"
              action={formAction}
            >
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Name:</Form.Label>
                <p
                  className="text-start mb-1 red-sec-text small-text"
                  style={nameError ? unhidden : hidden}
                >
                  Is empty or lenght is greater than 50.
                </p>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="name"
                  placeholder="name"
                  isInvalid={nameError}
                  onInput={(e) => {
                    if (
                      validators.lengthSmallerThen(e.target.value, 50) &&
                      validators.stringIsNotEmpty(e.target.value)
                    ) {
                      setNameError(false);
                    } else {
                      setNameError(true);
                    }
                  }}
                  maxLength={50}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Nip:</Form.Label>
                <p
                  className="text-start mb-1 red-sec-text small-text"
                  style={nipError ? unhidden : hidden}
                >
                  Is empty, not a number or lenght is greater than 15.
                </p>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="nip"
                  placeholder="nip"
                  isInvalid={nipError}
                  onInput={(e) => {
                    if (
                      validators.lengthSmallerThen(e.target.value, 12) &&
                      validators.stringIsNotEmpty(e.target.value) &&
                      validators.haveOnlyNumbers(e.target.value)
                    ) {
                      setNipError(false);
                    } else {
                      setNipError(true);
                    }
                  }}
                  maxLength={15}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Street:</Form.Label>
                <p
                  className="text-start mb-1 red-sec-text small-text"
                  style={streetError ? unhidden : hidden}
                >
                  Is empty or lenght is greater than 200.
                </p>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="street"
                  placeholder="street"
                  isInvalid={streetError}
                  onInput={(e) => {
                    if (
                      validators.lengthSmallerThen(e.target.value, 200) &&
                      validators.stringIsNotEmpty(e.target.value)
                    ) {
                      setStreetError(false);
                    } else {
                      setStreetError(true);
                    }
                  }}
                  maxLength={200}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">City:</Form.Label>
                <p
                  className="text-start mb-1 red-sec-text small-text"
                  style={cityError ? unhidden : hidden}
                >
                  Is empty or lenght is greater than 200.
                </p>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="city"
                  placeholder="city"
                  isInvalid={cityError}
                  onInput={(e) => {
                    if (
                      validators.lengthSmallerThen(e.target.value, 200) &&
                      validators.stringIsNotEmpty(e.target.value)
                    ) {
                      setCityError(false);
                    } else {
                      setCityError(true);
                    }
                  }}
                  maxLength={200}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Postal code:</Form.Label>
                <p
                  className="text-start mb-1 red-sec-text small-text"
                  style={postalError ? unhidden : hidden}
                >
                  Is empty or lenght is greater than 25.
                </p>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="postal"
                  placeholder="postal code"
                  isInvalid={postalError}
                  onInput={(e) => {
                    if (
                      validators.lengthSmallerThen(e.target.value, 25) &&
                      validators.stringIsNotEmpty(e.target.value)
                    ) {
                      setPostalError(false);
                    } else {
                      setPostalError(true);
                    }
                  }}
                  maxLength={25}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">
                  Credit Limit:
                </Form.Label>
                <p
                  className="text-start mb-1 red-sec-text small-text"
                  style={postalError ? unhidden : hidden}
                >
                  Is empty or lenght is greater than 25.
                </p>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  name="credit"
                  placeholder="credit limit"
                  isInvalid={postalError}
                  onInput={(e) => {
                    if (validators.haveOnlyNumbers(e.target.value)) {
                      setPostalError(false);
                    } else {
                      setPostalError(true);
                    }
                  }}
                  maxLength={25}
                />
              </Form.Group>
              <Form.Group className="mb-4">
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
              className="mb-5 mt-2"
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
            state.error = false;
            state.completed = false;
            state.message = "";
            setIsLoading(false);
          }}
        />
        <Toastes.SuccessToast
          showToast={state.completed && !state.error}
          message={state.message}
          onHideFun={() => {
            state.error = false;
            state.completed = false;
            state.message = "";
            setIsLoading(false);
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
}

AddClientOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
};

export default AddClientOffcanvas;
