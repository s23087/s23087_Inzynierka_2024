"use client";

import ErrorMessage from "@/components/smaller_components/error_message";
import changeTypeToOrganization from "@/utils/settings/switch_type";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { Form, Container, Button, Row, Col } from "react-bootstrap";

/**
 * Generate page that includes form that allow to change app type from solo to organization.
 */
export default function ChangeAppType() {
  const router = useRouter();
  // True if modify action is running
  const [isLoading, setIsLoading] = useState(false);
  // Form error
  const [errorMessage, setErrorMessage] = useState("");
  return (
    <Container className="px-4 pt-4" fluid>
      <Form className="mx-1 mx-xl-3">
        <ErrorMessage
          message={errorMessage}
          messageStatus={errorMessage !== ""}
        />
        <p className="blue-main-text text-center text-sm-start mb-1">
          Are you sure that you want to change from solo type to organization
          type? The change will be permanent.
        </p>
        <p className="blue-main-text text-center text-sm-start mb-1">
          If you accept you will be logout.
        </p>
        <Container className="p-0" fluid>
          <Row>
            <Col xs="12" sm="6" lg="5" xl="3" xxl="2">
              <Button
                className="mt-3 py-3 w-100"
                variant="mainBlue"
                onClick={async (e) => {
                  e.preventDefault();
                  setIsLoading(true);
                  let result = await changeTypeToOrganization();
                  if (!result.error) {
                    setIsLoading(false);
                    setErrorMessage("");
                    router.push("/");
                  } else {
                    setIsLoading(false);
                    setErrorMessage(result.message);
                  }
                }}
              >
                {isLoading ? (
                  <div className="spinner-border main-text"></div>
                ) : (
                  "Yes switch to organization type"
                )}
              </Button>
            </Col>
            <Col xs="12" sm="6" lg="5" xl="3" xxl="2">
              <Button
                className="mt-3 py-3 w-100"
                variant="secBlue"
                type="Click"
                onClick={() => {
                  router.push(".");
                }}
              >
                Go back
              </Button>
            </Col>
          </Row>
        </Container>
      </Form>
    </Container>
  );
}
