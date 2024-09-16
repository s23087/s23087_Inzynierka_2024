"use client";

import PropTypes from "prop-types";
import { useRouter } from "next/navigation";
import { useFormState } from "react-dom";
import { Form, Container, Button, Stack } from "react-bootstrap";
import { useState } from "react";
import modifyClient from "@/utils/flexible/modify_client";
import Toastes from "@/components/smaller_components/toast";
import StringValidtor from "@/utils/validators/form_validator/stringValidator";

function ModifyUserOrgForm({ orgInfo, countries }) {
  const router = useRouter();
  const [orgNameError, setOrgNameError] = useState(false);
  const [nipMyError, setMyNipError] = useState(false);
  const [streetError, setStreetError] = useState(false);
  const [cityError, setCityError] = useState(false);
  const [postalError, setPostalError] = useState(false);
  const anyErrorActive =
    orgNameError || nipMyError || streetError || cityError || postalError;
  const [prevState] = useState({
    orgName: orgInfo.orgName,
    street: orgInfo.street,
    city: orgInfo.city,
    postalCode: orgInfo.postal,
  });
  const [isLoading, setIsLoading] = useState(false);
  const bindedFunc = modifyClient.bind(null, orgInfo.id).bind(null, prevState);
  const [state, formAction] = useFormState(bindedFunc, {
    error: false,
    message: "",
    completed: false,
  });
  const buttonStyle = {
    width: "220px",
    height: "55px",
  };
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
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
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={orgNameError ? unhidden : hidden}
          >
            Is empty or lenght is greater than 50.
          </p>
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="text"
            name="name"
            defaultValue={orgInfo.orgName}
            isInvalid={orgNameError}
            onInput={(e) => {
              StringValidtor.normalStringValidtor(
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
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={nipMyError ? unhidden : hidden}
          >
            Is empty, not a number or lenght is greater than 15.
          </p>
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="text"
            name="nip"
            defaultValue={orgInfo.nip}
            isInvalid={nipMyError}
            onInput={(e) => {
              StringValidtor.emptyNumberStringValidtor(
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
            defaultValue={orgInfo.street}
            isInvalid={streetError}
            onInput={(e) => {
              StringValidtor.normalStringValidtor(
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
            defaultValue={orgInfo.city}
            isInvalid={cityError}
            onInput={(e) => {
              StringValidtor.normalStringValidtor(
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
            defaultValue={orgInfo.postalCode}
            isInvalid={postalError}
            onInput={(e) => {
              StringValidtor.normalStringValidtor(
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
            {Object.values(countries)
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
            disabled={anyErrorActive}
            onClick={(e) => {
              e.preventDefault();
              if (anyErrorActive) return;
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
        message="You have successfuly modified org info."
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

ModifyUserOrgForm.PropTypes = {
  orgInfo: PropTypes.object.isRequired,
  countries: PropTypes.object.isRequired,
};

export default ModifyUserOrgForm;
