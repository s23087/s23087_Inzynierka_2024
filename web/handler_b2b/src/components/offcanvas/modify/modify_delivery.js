import Image from "next/image";
import PropTypes from "prop-types";
import {
  Offcanvas,
  Container,
  Row,
  Col,
  Button,
  Form,
  Stack,
} from "react-bootstrap";
import CloseIcon from "../../../../public/icons/close_black.png";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import SpecialInput from "@/components/smaller_components/special_input";
import AddDeliveryCompanyWindow from "@/components/windows/addCompany";
import getDeliveryCompany from "@/utils/deliveries/get_delivery_company";
import AddWaybillWindow from "@/components/windows/addWaybill";
import updateDelivery from "@/utils/deliveries/update_delivery";
import ErrorMessage from "@/components/smaller_components/error_message";

function ModifyDeliveryOffcanvas({
  showOffcanvas,
  hideFunction,
  isDeliveryToUser,
  delivery,
}) {
  const router = useRouter();
  // Get rest data
  useEffect(() => {
    if (showOffcanvas) {
      getDeliveryCompany().then((data) => {
        if (data !== null) {
          setCompaniesDownloadError(false);
          setDeliveryCompanies(data);
        } else {
          setCompaniesDownloadError(true);
        }
      });
      setWaybills(delivery.waybill);
      prevState.estimated = delivery.estimated.substring(0, 10);
    }
  }, [showOffcanvas]);
  const [deliveryCompanies, setDeliveryCompanies] = useState([]);
  const [companiesDownloadError, setCompaniesDownloadError] = useState(false);
  // Waybills
  const [waybills, setWaybills] = useState([]);
  const [isAddWaybillShow, setIsAddWaybillShow] = useState(false);
  const waybillExist = (variable) => {
    return waybills.findIndex((item) => item === variable) != -1;
  };
  const [rerenderVar, setRerenderVar] = useState(1);
  // PrevState
  const [prevState] = useState({
    estimated: "",
    isWaybillModified: false,
  });
  // Delivery company add
  const [isAddCompanyShow, setIsAddCompanyShow] = useState(false);
  // Error
  const isFormErrorActive = () => {
    return deliveryCompanies.length === 0 || companiesDownloadError;
  };
  // Loading element
  const [isLoading, setIsLoading] = useState(false);
  // Form action
  const [state, formAction] = useFormState(
    updateDelivery
      .bind(null, delivery.deliveryId)
      .bind(null, waybills)
      .bind(null, isDeliveryToUser)
      .bind(null, prevState),
    {
      error: false,
      completed: false,
      message: "",
    },
  );
  // Styles
  const maxStyle = {
    maxWidth: "393px",
  };
  const vhStyle = {
    height: "81vh",
  };
  const buttonStyle = {
    maxWidth: "250px",
    borderRadius: "5px",
  };
  return (
    <Offcanvas
      className="h-100 minScalableWidth"
      show={showOffcanvas}
      onHide={hideFunction}
      placement="bottom"
    >
      <Container className="h-100 w-100 p-0" fluid>
        <Offcanvas.Header className="border-bottom-grey px-xl-5">
          <Container className="px-3" fluid>
            <Row>
              <Col xs="9" className="d-flex align-items-center">
                <p className="blue-main-text h4 mb-0">
                  Modify delivery: {delivery.deliveryId}
                </p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                    if (!state.error && state.complete) {
                      router.refresh();
                    }
                  }}
                  className="pe-0"
                >
                  <Image src={CloseIcon} alt="Close" />
                </Button>
              </Col>
            </Row>
          </Container>
        </Offcanvas.Header>
        <Offcanvas.Body className="px-4 px-xl-5 pb-0" as="div">
          <Container className="p-0" style={vhStyle} fluid>
            <Form
              className="mx-1 mx-xl-3 ps-1"
              id="createDelivery"
              action={formAction}
            >
              <ErrorMessage
                message="Could not download delivery companies."
                messageStatus={companiesDownloadError}
              />
              <Form.Group className="mb-3">
                <Form.Label className="blue-main-text">Proforma:</Form.Label>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="text"
                  defaultValue={delivery.proforma}
                  readOnly
                  disabled
                />
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">
                  Estimated delivery date:
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm maxInputWidth"
                  type="date"
                  name="estimated"
                  defaultValue={new Date(delivery.estimated).toLocaleDateString(
                    "en-CA",
                  )}
                />
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">
                  Delivery company:
                </Form.Label>
                <Form.Select
                  className="input-style shadow-sm maxInputWidth"
                  name="company"
                >
                  <option value={-1}>{delivery.deliveryCompany}</option>
                  {Object.values(deliveryCompanies)
                    .filter((e) => e.name !== delivery.deliveryCompany)
                    .map((value) => {
                      return (
                        <option key={value.id} value={value.id}>
                          {value.name}
                        </option>
                      );
                    })}
                </Form.Select>
                <Button
                  variant="mainBlue"
                  className="mb-2 mt-4"
                  onClick={() => setIsAddCompanyShow(true)}
                >
                  Add new company
                </Button>
                <AddDeliveryCompanyWindow
                  modalShow={isAddCompanyShow}
                  onHideFunction={() => setIsAddCompanyShow(false)}
                  companies={deliveryCompanies}
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Stack key={rerenderVar}>
                  <Form.Label className="blue-main-text">Waybills:</Form.Label>
                  {waybills.map((value, key) => {
                    return (
                      <SpecialInput
                        value={value}
                        key={value}
                        deleteValue={() => {
                          waybills.splice(key, 1);
                          if (rerenderVar === 1) {
                            setRerenderVar(rerenderVar + 1);
                          }
                          if (rerenderVar > 1) {
                            setRerenderVar(rerenderVar - 1);
                          }
                          prevState.isWaybillModified = true;
                        }}
                        existFun={waybillExist}
                        validatorFunc={(val) =>
                          val.length > 0 && val.length <= 40
                        }
                        errorMessage="Is empty or excced 40 chars."
                        modifyValue={(variable) => {
                          waybills[key] = variable;
                          prevState.isWaybillModified = true;
                        }}
                      />
                    );
                  })}
                  <AddWaybillWindow
                    modalShow={isAddWaybillShow}
                    onHideFunction={() => setIsAddWaybillShow(false)}
                    addAction={(variable) => {
                      waybills.push(variable);
                      prevState.isWaybillModified = true;
                    }}
                    waybillExist={waybillExist}
                  />
                  <Button
                    variant="mainBlue"
                    style={buttonStyle}
                    className="mb-3 mt-4 ms-3 py-3"
                    onClick={() => setIsAddWaybillShow(true)}
                  >
                    Add Waybill
                  </Button>
                </Stack>
              </Form.Group>
              <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
                <Row style={maxStyle} className="mx-auto minScalableWidth">
                  <Col>
                    <Button
                      variant="mainBlue"
                      className="w-100"
                      type="submit"
                      disabled={isFormErrorActive()}
                      onClick={(e) => {
                        e.preventDefault();
                        setIsLoading(true);

                        let form = document.getElementById("createDelivery");
                        form.requestSubmit();
                      }}
                    >
                      {isLoading && !state.completed ? (
                        <div className="spinner-border main-text"></div>
                      ) : (
                        "Save"
                      )}
                    </Button>
                  </Col>
                  <Col>
                    <Button
                      variant="red"
                      className="w-100"
                      onClick={() => {
                        hideFunction();
                        if (!state.error && state.completed) {
                          router.refresh();
                        }
                      }}
                    >
                      Cancel
                    </Button>
                  </Col>
                </Row>
              </Container>
            </Form>
          </Container>
        </Offcanvas.Body>

        <Toastes.ErrorToast
          showToast={state.completed && state.error}
          message={state.message}
          onHideFun={() => {
            resetState();
          }}
        />
        <Toastes.SuccessToast
          showToast={state.completed && !state.error}
          message={state.message}
          onHideFun={() => {
            resetState();
            hideFunction();
            router.refresh();
          }}
        />
      </Container>
    </Offcanvas>
  );

  function resetState() {
    state.error = false;
    state.completed = false;
    state.message = "";
    setIsLoading(false);
  }
}

ModifyDeliveryOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  isDeliveryToUser: PropTypes.bool.isRequired,
  delivery: PropTypes.object.isRequired,
};

export default ModifyDeliveryOffcanvas;
