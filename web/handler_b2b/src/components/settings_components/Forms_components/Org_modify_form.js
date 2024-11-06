"use client";

import PropTypes from "prop-types";
import { useRouter } from "next/navigation";
import { useFormState } from "react-dom";
import { Form, Container, Button, Stack } from "react-bootstrap";
import { useState } from "react";
import modifyClient from "@/utils/flexible/modify_client";
import Toastes from "@/components/smaller_components/toast";
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import ErrorMessage from "@/components/smaller_components/error_message";

/**
 * Return form that allow user to change their data.
 * @component
 * @param {object} props Component props
 * @param {{id: Number, orgName: string, street: string, city: string, nip: Number|undefined, postal: string, countryId: Number, country: string}} props.orgInfo Object containing organization information.
 * @param {Array<{id: Number, countryName: string}>} props.countries List of object describing countries.
 * @return {JSX.Element} Container element
 */
function ModifyUserOrgForm({ orgInfo, countries }) {
  const router = useRouter();
  // Form errors
  const [orgNameError, setOrgNameError] = useState(false);
  const [nipMyError, setMyNipError] = useState(false);
  const [streetError, setStreetError] = useState(false);
  const [cityError, setCityError] = useState(false);
  const [postalError, setPostalError] = useState(false);
  // Check if form can be submitted
  const isFormErrorActive = () =>
    orgNameError ||
    nipMyError ||
    streetError ||
    cityError ||
    postalError ||
    !countries ||
    orgInfo.street === "connection error";
  // Previous state of organization data
  const [prevState] = useState({
    orgName: orgInfo.orgName,
    street: orgInfo.street,
    city: orgInfo.city,
    postalCode: orgInfo.postal,
  });
  // Set true when form action is activated and false when no action happen
  const [isLoading, setIsLoading] = useState(false);
  // Form function
  const boundFunc = modifyClient.bind(null, orgInfo.id).bind(null, prevState);
  const [state, formAction] = useFormState(boundFunc, {
    error: false,
    message: "",
    completed: false,
  });
  // Styles
  const buttonStyle = {
    width: "220px",
    height: "55px",
  };
  return (
    <Container className="px-4 pt-4" fluid>
      <Form className="mx-1 mx-xl-4" id="UserOrgModify" action={formAction}>
        <Form.Group className="mb-3">
          <Form.Label className="blue-main-text h5">
            Change organization data
          </Form.Label>
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label className="blue-main-text">Name:</Form.Label>
          <ErrorMessage
            message="Is empty or length is greater than 50."
            messageStatus={orgNameError}
          />
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="text"
            name="name"
            defaultValue={orgInfo.orgName}
            isInvalid={orgNameError}
            onInput={(e) => {
              InputValidator.normalStringValidator(
                e.target.value,
                setOrgNameError,
                50,
              );
            }}
            maxLength={50}
          />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label className="blue-main-text">Nip:</Form.Label>
          <ErrorMessage
            message="Is empty, not a number or length is greater than 15."
            messageStatus={nipMyError}
          />
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="text"
            name="nip"
            defaultValue={orgInfo.nip}
            isInvalid={nipMyError}
            onInput={(e) => {
              InputValidator.emptyNumberStringValidator(
                e.target.value,
                setMyNipError,
                15,
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
            defaultValue={orgInfo.street}
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
            defaultValue={orgInfo.city}
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
            defaultValue={orgInfo.postalCode}
            isInvalid={postalError}
            onInput={(e) => {
              InputValidator.normalStringValidator(
                e.target.value,
                setPostalError,
                25,
              );
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
            <option key={orgInfo.countryId} value={orgInfo.countryId}>
              {orgInfo.country}
            </option>
            {Object.values(countries ?? [])
              .filter((e) => e.id !== orgInfo.countryId)
              .map((value) => {
                return (
                  <option key={value.id} value={value.id}>
                    {value.countryName}
                  </option>
                );
              })}
          </Form.Select>
        </Form.Group>
        <Stack>
          <Button
            className="mt-3 mx-auto ms-sm-0"
            variant="mainBlue"
            type="Submit"
            style={buttonStyle}
            disabled={isFormErrorActive()}
            onClick={(e) => {
              e.preventDefault();
              if (isFormErrorActive()) return;
              setIsLoading(true);
              let form = document.getElementById("UserOrgModify");
              form.requestSubmit();
            }}
          >
            {isLoading && !state.completed ? (
              <div className="spinner-border main-text"></div>
            ) : (
              "Save Changes"
            )}
          </Button>
        </Stack>
      </Form>
      <Stack className="ms-sm-1 ms-xl-4">
        <Button
          className="mt-3 mx-auto ms-sm-0"
          variant="secBlue"
          type="Click"
          style={buttonStyle}
          onClick={(e) => {
            e.preventDefault();
            router.push(".");
          }}
        >
          Go back
        </Button>
      </Stack>
      <Toastes.ErrorToast
        showToast={state.completed && state.error}
        message={state.message}
        onHideFun={() => {
          setIsLoading(false);
          state.error = false;
          state.message = "";
          state.completed = false;
          router.refresh();
        }}
      />
      <Toastes.SuccessToast
        showToast={state.completed && !state.error}
        message="You have successfully modified org info."
        onHideFun={() => {
          state.error = false;
          state.completed = false;
          state.message = "";
          setIsLoading(false);
          router.refresh();
        }}
      />
    </Container>
  );
}

ModifyUserOrgForm.propTypes = {
  orgInfo: PropTypes.object.isRequired,
  countries: PropTypes.object.isRequired,
};

export default ModifyUserOrgForm;
