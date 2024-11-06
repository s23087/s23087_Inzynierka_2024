"use client";

import { useRouter } from "next/navigation";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import { Form, Container, Button, Stack } from "react-bootstrap";
import AddUser from "@/utils/settings/add_user";
import Toastes from "@/components/smaller_components/toast";
import getRoles from "@/utils/roles/get_roles";
import ErrorMessage from "@/components/smaller_components/error_message";
import InputValidator from "@/utils/validators/form_validator/inputValidator";

export default function AddUserPage() {
  const router = useRouter();
  // download data holder
  const [roles, setRoles] = useState({});
  // True if download error occurred
  const [roleDownloadError, setRoleDownloadError] = useState(false);
  // download data
  useEffect(() => {
    getRoles().then((data) => {
      if (data !== null) {
        setRoleDownloadError(false);
        setRoles(data);
      } else {
        setRoleDownloadError(true);
      }
    });
  }, []);
  // Form error
  const [emailError, setEmailError] = useState(false);
  const [nameError, setNameError] = useState(false);
  const [surnameError, setSurnameError] = useState(false);
  const [passError, setPassError] = useState(false);
  /**
   * Checks if form can be submitted
   * @return {boolean}
   */
  let isFormErrorActive = () =>
    emailError || nameError || surnameError || passError || roleDownloadError;
  // True if add user action is running
  const [isLoading, setIsLoading] = useState(false);
  // form action
  const [state, formAction] = useFormState(AddUser, {
    error: false,
    completed: false,
    message: "",
  });
  // styles
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
        <ErrorMessage
          message="Could not download role list."
          messageStatus={roleDownloadError}
        />
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">Email:</Form.Label>
          <Form.Control
            className="input-style shadow-sm"
            type="email"
            name="email"
            maxLength={350}
            onInput={(e) => {
              InputValidator.emailValidator(e.target.value, setEmailError, 350);
            }}
            isInvalid={emailError}
            placeholder="email"
          />
          <ErrorMessage message="Incorrect email." messageStatus={emailError} />
        </Form.Group>
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">Name:</Form.Label>
          <Form.Control
            className="input-style shadow-sm"
            type="text"
            name="name"
            isInvalid={nameError}
            maxLength={250}
            onInput={(e) => {
              InputValidator.normalStringValidator(
                e.target.value,
                setNameError,
                250,
              );
            }}
            placeholder="name"
          />
          <ErrorMessage
            message="Incorrect username. Cannot be empty or exceed 250 chars."
            messageStatus={nameError}
          />
        </Form.Group>
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">Surname:</Form.Label>
          <Form.Control
            className="input-style shadow-sm"
            type="text"
            name="surname"
            isInvalid={surnameError}
            maxLength={250}
            onInput={(e) => {
              InputValidator.normalStringValidator(
                e.target.value,
                setSurnameError,
                250,
              );
            }}
            placeholder="surname"
          />
          <ErrorMessage
            message="Incorrect surname. Cannot be empty or exceed 250 chars."
            messageStatus={surnameError}
          />
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
              InputValidator.isEmptyValidator(e.target.value, setPassError);
            }}
          />
          <ErrorMessage message="Cannot be empty." messageStatus={passError} />
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
            disabled={isFormErrorActive()}
            onClick={(e) => {
              e.preventDefault();
              if (isFormErrorActive()) return;
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
