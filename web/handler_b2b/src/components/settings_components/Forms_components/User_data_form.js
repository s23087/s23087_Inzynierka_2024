"use client";

import PropTypes from "prop-types";
import { useRouter } from "next/navigation";
import { useFormState } from "react-dom";
import { Form, Container, Button, Stack } from "react-bootstrap";
import validators from "@/utils/validators/validator";
import { useState } from "react";
import changeUserInfo from "@/utils/settings/change_user_info";
import Toastes from "@/components/smaller_components/toast";
import StringValidtor from "@/utils/validators/form_validator/stringValidator";

function ModifyUserForm({ email, name, surname }) {
  const router = useRouter();
  const [emailError, setEmailError] = useState(false);
  const [nameError, setNameError] = useState(false);
  const [surnameError, setSurnameError] = useState(false);
  const anyError = emailError || nameError || surnameError;
  const [isLoading, setIsLoading] = useState(false);
  const [prevState] = useState({
    email: email,
    name: name,
    surname: surname,
  });
  const [state, formAction] = useFormState(
    changeUserInfo.bind(null, prevState),
    {
      error: false,
      completed: false,
      message: "",
    },
  );
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
      <Form
        className="mx-1 mx-xl-4"
        action={formAction}
        id="changeUserInfoForm"
      >
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
              StringValidtor.emailValidator(e.target.value, setEmailError, 350)
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
            Is empty or lenght is greater than 250.
          </p>
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="text"
            name="name"
            defaultValue={name}
            isInvalid={nameError}
            onInput={(e) => {
              StringValidtor.normalStringValidtor(e.target.value, setNameError, 250)
            }}
            maxLength={250}
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
              StringValidtor.normalStringValidtor(e.target.value, setSurnameError, 200)
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
            onClick={(e) => {
              e.preventDefault();
              if (anyError) return;
              setIsLoading(true);
              let form = document.getElementById("changeUserInfoForm");
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
      <Stack className="px-1 px-xl-4">
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
      <Toastes.SuccessToast
        showToast={state.completed && !state.error}
        message={state.message}
        onHideFun={() => {
          state.error = false;
          state.completed = false;
          state.message = "";
          setIsLoading(false);
          router.refresh();
        }}
      />
      <Toastes.ErrorToast
        showToast={state.completed && state.error}
        message={state.message}
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

ModifyUserForm.PropTypes = {
  email: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  surname: PropTypes.string.isRequired,
};

export default ModifyUserForm;
