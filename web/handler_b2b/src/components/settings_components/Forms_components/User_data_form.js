"use client";

import PropTypes from "prop-types";
import { useRouter } from "next/navigation";
import { Form, Container, Button, Stack } from "react-bootstrap";
import validators from "@/utils/validators/validator";
import { useState } from "react";

function ModifyUserForm({ email, name, surname }) {
  const router = useRouter();
  const [emailError, setEmailError] = useState(false);
  const [nameError, setNameError] = useState(false);
  const [surnameError, setSurnameError] = useState(false);
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
            Change your user data
          </Form.Label>
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label className="blue-main-text">Email:</Form.Label>
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={emailError ? unhidden : hidden}
          >
            Is empty or not a email.
          </p>
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="email"
            name="email"
            defaultValue={email}
            isInvalid={emailError}
            onInput={(e) => {
              if (
                validators.lengthSmallerThen(e.target.value, 350) &&
                validators.isEmail(e.target.value) &&
                validators.stringIsNotEmpty(e.target.value)
              ) {
                setEmailError(false);
              } else {
                setEmailError(true);
              }
            }}
            maxLength={350}
          />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label className="blue-main-text">Name:</Form.Label>
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={nameError ? unhidden : hidden}
          >
            Is empty, not a number or lenght is greater than 15.
          </p>
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="text"
            name="name"
            defaultValue={name}
            isInvalid={nameError}
            onInput={(e) => {
              if (
                validators.lengthSmallerThen(e.target.value, 12) &&
                validators.stringIsNotEmpty(e.target.value) &&
                validators.haveOnlyNumbers(e.target.value)
              ) {
                setNameError(false);
              } else {
                setNameError(true);
              }
            }}
            maxLength={15}
          />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label className="blue-main-text">Surname:</Form.Label>
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={surnameError ? unhidden : hidden}
          >
            Is empty or lenght is greater than 200.
          </p>
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="text"
            name="surname"
            defaultValue={surname}
            isInvalid={surnameError}
            onInput={(e) => {
              if (
                validators.lengthSmallerThen(e.target.value, 200) &&
                validators.stringIsNotEmpty(e.target.value)
              ) {
                setSurnameError(false);
              } else {
                setSurnameError(true);
              }
            }}
            maxLength={200}
          />
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

ModifyUserForm.PropTypes = {
  email: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  surname: PropTypes.string.isRequired,
};

export default ModifyUserForm;
