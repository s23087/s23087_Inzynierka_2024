"use client";

import { useRouter } from "next/navigation";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import { Form, Container, Button, Stack } from "react-bootstrap";
import AddUser from "@/utils/settings/add_user";
import validators from "@/utils/validators/validator";
import Toastes from "@/components/smaller_components/toast";
import getRoles from "@/utils/roles/get_roles";

export default function AddUserPage() {
  const router = useRouter();
  const [roles, setRoles] = useState({});
  useEffect(() => {
    const roles = getRoles();
    roles.then((data) => setRoles(data));
  }, []);
  const [emailError, setEmailError] = useState(false);
  const [nameError, setNameError] = useState(false);
  const [surnameError, setSurnameError] = useState(false);
  const [passError, setPassError] = useState(false);
  let anyError = emailError || nameError || surnameError || passError;
  const [isLoading, setIsLoading] = useState(false);
  const [state, formAction] = useFormState(AddUser, {
    error: false,
    completed: false,
    message: "",
  });
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };
  const buttonStyle = {
    width: "220px",
    height: "55px",
  };
  const maxInputWidth = {
    maxWidth: "519px",
  };
  return (
    <Container className="px-4 pt-4" fluid>
      <Form className="mx-1 mx-xl-4" id="addUserForm" action={formAction}>
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">Email:</Form.Label>
          <Form.Control
            className="input-style shadow-sm"
            type="email"
            name="email"
            onInput={(e) => {
              if (
                validators.lengthSmallerThen(e.target.value, 350) &&
                validators.stringIsNotEmpty(e.target.value) &&
                validators.isEmail(e.target.value)
              ) {
                setEmailError(false);
              } else {
                setEmailError(true);
              }
            }}
            isInvalid={emailError}
            placeholder="email"
          />
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={emailError ? unhidden : hidden}
          >
            Incorrect email.
          </p>
        </Form.Group>
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">Name:</Form.Label>
          <Form.Control
            className="input-style shadow-sm"
            type="text"
            name="name"
            isInvalid={nameError}
            onInput={(e) => {
              if (
                validators.lengthSmallerThen(e.target.value, 250) &&
                validators.stringIsNotEmpty(e.target.value)
              ) {
                setNameError(false);
              } else {
                setNameError(true);
              }
            }}
            placeholder="name"
          />
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={nameError ? unhidden : hidden}
          >
            Incorrect username. Cannot be empty or excceed 250 chars.
          </p>
        </Form.Group>
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">Surname:</Form.Label>
          <Form.Control
            className="input-style shadow-sm"
            type="text"
            name="surname"
            isInvalid={surnameError}
            onInput={(e) => {
              if (
                validators.lengthSmallerThen(e.target.value, 250) &&
                validators.stringIsNotEmpty(e.target.value)
              ) {
                setSurnameError(false);
              } else {
                setSurnameError(true);
              }
            }}
            placeholder="surname"
          />
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={surnameError ? unhidden : hidden}
          >
            Incorrect surname. Cannot be empty or excceed 250 chars.
          </p>
        </Form.Group>
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">Password:</Form.Label>
          <Form.Control
            className="input-style shadow-sm"
            type="password"
            name="password"
            placeholder="password"
            isInvalid={passError}
            onInput={(e) => {
              if (validators.stringIsNotEmpty(e.target.value)) {
                setPassError(false);
              } else {
                setPassError(true);
              }
            }}
          />
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={passError ? unhidden : hidden}
          >
            Cannot be empty.
          </p>
        </Form.Group>
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">Role:</Form.Label>
          <Form.Select className="input-style shadow-sm" name="role">
            {Object.values(roles).map((value) => {
              return (
                <option key={value} value={value}>
                  {value}
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
            onClick={(e) => {
              e.preventDefault();
              if (anyError) return;
              setIsLoading(true);
              let form = document.getElementById("addUserForm");
              form.requestSubmit();
            }}
          >
            {isLoading && !state.completed ? (
              <div className="spinner-border main-text"></div>
            ) : (
              "Create user"
            )}
          </Button>
        </Stack>
      </Form>
      <Stack className="mx-1 mx-xl-4">
        <Button
          className="mt-3 mx-auto ms-sm-0"
          variant="secBlue"
          type="Click"
          style={buttonStyle}
          onClick={() => {
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
          state.error = false;
          state.completed = false;
          state.message = "";
          setIsLoading(false);
          router.refresh();
        }}
      />
      <Toastes.SuccessToast
        showToast={state.completed && !state.error}
        message={state.message}
        onHideFun={() => {
          state.error = false;
          state.completed = false;
          state.message = "";
          let form = document.getElementById("addUserForm");
          form.reset();
          setIsLoading(false);
          router.refresh();
        }}
      />
    </Container>
  );
}
