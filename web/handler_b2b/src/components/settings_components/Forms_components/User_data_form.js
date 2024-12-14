"use client";

import PropTypes from "prop-types";
import { useRouter } from "next/navigation";
import { useFormState } from "react-dom";
import { Form, Container, Button, Stack } from "react-bootstrap";
import { useState } from "react";
import changeUserInfo from "@/utils/settings/change_user_info";
import Toastes from "@/components/smaller_components/toast";
import InputValidator from "@/utils/validators/form_validator/inputValidator";
import ErrorMessage from "@/components/smaller_components/error_message";

/**
 * Return form that allow user to change their data.
 * @component
 * @param {object} props Component props
 * @param {string} props.email Current user email.
 * @param {string} props.name Current user name.
 * @param {string} props.surname Current user surname.
 * @return {JSX.Element} Container element
 */
function ModifyUserForm({ email, name, surname }) {
  const router = useRouter();
  // Error
  const [emailError, setEmailError] = useState(false);
  const [nameError, setNameError] = useState(false);
  const [surnameError, setSurnameError] = useState(false);
  // Check if any error is active
  const isFormErrorActive = () => emailError || nameError || surnameError;
  // Set true if component is loading
  const [isLoading, setIsLoading] = useState(false);
  // Previous state of user data
  const [prevState] = useState({
    email: email,
    name: name,
    surname: surname,
  });
  // Form action
  const [state, formAction] = useFormState(
    changeUserInfo.bind(null, prevState),
    {
      error: false,
      completed: false,
      message: "",
    },
  );
  // Styles
  const buttonStyle = {
    width: "220px",
    height: "55px",
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
          <ErrorMessage
            message="Is empty or not a email."
            messageStatus={emailError}
          />
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="email"
            name="email"
            defaultValue={email}
            isInvalid={emailError}
            onInput={(e) => {
              InputValidator.emailValidator(e.target.value, setEmailError, 350);
            }}
            maxLength={350}
          />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label className="blue-main-text">Name:</Form.Label>
          <ErrorMessage
            message="Is empty or length is greater than 250."
            messageStatus={nameError}
          />
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="text"
            name="name"
            defaultValue={name}
            isInvalid={nameError}
            onInput={(e) => {
              InputValidator.normalStringValidator(
                e.target.value,
                setNameError,
                250,
              );
            }}
            maxLength={250}
          />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label className="blue-main-text">Surname:</Form.Label>
          <ErrorMessage
            message="Is empty or length is greater than 200."
            messageStatus={surnameError}
          />
          <Form.Control
            className="input-style shadow-sm maxInputWidth"
            type="text"
            name="surname"
            defaultValue={surname}
            isInvalid={surnameError}
            onInput={(e) => {
              InputValidator.normalStringValidator(
                e.target.value,
                setSurnameError,
                200,
              );
            }}
            maxLength={200}
          />
        </Form.Group>
        <Stack>
          <Button
            className="mt-3 mx-auto ms-sm-0"
            variant="mainBlue"
            type="Submit"
            disabled={isFormErrorActive()}
            style={buttonStyle}
            onClick={(e) => {
              e.preventDefault();
              if (isFormErrorActive()) return;
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
      <Stack className="px-1 px-xl-4 pb-5">
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

ModifyUserForm.propTypes = {
  email: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  surname: PropTypes.string.isRequired,
};

export default ModifyUserForm;
