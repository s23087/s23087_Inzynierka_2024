"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { useFormState } from "react-dom";
import { Form, Container, Button, Stack } from "react-bootstrap";
import ChangePassword from "@/utils/settings/change_password";
import validators from "@/utils/validators/validator";
import ForceLogutWindow from "@/components/windows/force_logout";

export default function ChangePasswordPage() {
  const router = useRouter();
  const [isInvalid, setIsInvalid] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [state, formAction] = useFormState(ChangePassword, {
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
      <Form className="mx-1 mx-xl-4" id="changePassForm" action={formAction}>
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">Old Password:</Form.Label>
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={state.error ? unhidden : hidden}
          >
            {state.message}
          </p>
          <p
            className="text-start mb-1 red-sec-text small-text"
            style={isInvalid ? unhidden : hidden}
          >
            One of password are empty or password are the same.
          </p>
          <Form.Control
            className="input-style shadow-sm"
            type="password"
            name="oldPassword"
            onInput={(e) => {
              if (validators.stringIsNotEmpty(e.target.value)) {
                setIsInvalid(false);
              } else {
                setIsInvalid(true);
              }
            }}
            isInvalid={isInvalid}
            placeholder="old password"
            id="oldPassword"
          />
        </Form.Group>
        <Form.Group className="mb-3" style={maxInputWidth}>
          <Form.Label className="blue-main-text">New Password:</Form.Label>
          <Form.Control
            className="input-style shadow-sm"
            type="password"
            name="newPassword"
            id="newPassword"
            isInvalid={isInvalid}
            onInput={(e) => {
              if (validators.stringIsNotEmpty(e.target.value)) {
                setIsInvalid(false);
              } else {
                setIsInvalid(true);
              }
            }}
            placeholder="new password"
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
              let oldPass = document.getElementById("oldPassword");
              let newPass = document.getElementById("newPassword");
              if (newPass.value === oldPass.value) {
                setIsInvalid(true);
                return;
              }
              if (
                !validators.stringIsNotEmpty(newPass.value) ||
                !validators.stringIsNotEmpty(oldPass.value)
              ) {
                setIsInvalid(true);
                return;
              }
              setIsLoading(true);
              let form = document.getElementById("changePassForm");
              form.requestSubmit();
            }}
          >
            {isLoading && !state.error ? (
              <div className="spinner-border main-text"></div>
            ) : (
              "Change password"
            )}
          </Button>
        </Stack>
      </Form>
      <Stack>
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
      <ForceLogutWindow modalShow={!state.error && state.completed} />
    </Container>
  );
}