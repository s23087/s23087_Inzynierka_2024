"use client";

import { useRouter } from "next/navigation";
import { Form, Container, Button, Stack } from "react-bootstrap";
import CountryOptions from "@/components/registration/country_options";
import validators from "@/utils/validators/validator";
import { useState } from "react";

export default function ChangeOrgPage() {
  const router = useRouter();
  const [nameError, setNameError] = useState(false);
  const [nipError, setNipError] = useState(false);
  const [streetError, setStreetError] = useState(false);
  const [cityError, setCityError] = useState(false);
  const [postalError, setPostalError] = useState(false);
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
      <Form className="mx-1 mx-xl-4">
        <Form.Group className="mb-3">
          <Form.Label className="blue-main-text h5">
            Change organization data
          </Form.Label>
        </Form.Group>
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
            defaultValue="<<name>>"
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
            defaultValue="<<nip>>"
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
            defaultValue="<<street>>"
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
            defaultValue="<<city>>"
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
            defaultValue="<<postal code>>"
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
        <Form.Group className="mb-4">
          <Form.Label className="blue-main-text">Country:</Form.Label>
          <Form.Select
            id="countrySelect"
            className="input-style shadow-sm"
            name="country"
          >
            <CountryOptions />
          </Form.Select>
        </Form.Group>
        <Stack>
          <Button
            className="mt-3 mx-auto ms-sm-0"
            variant="mainBlue"
            type="Submit"
            style={buttonStyle}
          >
            Save changes
          </Button>
        </Stack>
      </Form>
      <Stack>
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
    </Container>
  );
}
