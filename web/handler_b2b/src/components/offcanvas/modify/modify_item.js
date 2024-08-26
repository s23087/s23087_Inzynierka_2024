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
import { useState } from "react";
import { useFormState } from "react-dom";
import createItem from "@/utils/warehouse/create_item";
import SpecialInput from "@/components/smaller_components/special_input";
import BindingInput from "@/components/smaller_components/binding_input";
import AddEanWindow from "@/components/windows/addEan";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import CloseIcon from "../../../../public/icons/close_black.png";
import switch_product_view from "../../../../public/icons/switch_product_view.png";
import switch_binding_view from "../../../../public/icons/switch_binding_view.png";

function ModifyItemOffcanvas({ showOffcanvas, hideFunction, item }) {
  const router = useRouter();
  const [isProductView, setIsProductView] = useState(true);
  const [eans, setEans] = useState(item.eans);
  const [bindings] = useState([
    {
      username: "user name",
      qty: "qty",
      price: "price",
      currency: "PLN",
      invoiceNumber: "invoice number",
    },
  ]);
  const [rerenderVar, setRerenderVar] = useState(1);
  const eanExist = (variable) => {
    return eans.findIndex((item) => item === variable) != -1;
  };
  const [isAddEanShow, setIsAddEanShow] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [showErrorToast, setShowErrorToast] = useState(false);
  const [showSuccessToast, setShowSuccessToast] = useState(false);
  const [state, formAction] = useFormState(createItem.bind(null, eans), {
    error: false,
    complete: false,
  });
  const maxStyle = {
    maxWidth: "393px",
  };
  const buttonStyle = {
    maxWidth: "250px",
    borderRadius: "5px",
  };
  const hidden = {
    display: "none",
    height: "81vh",
  };
  const unhidden = {
    display: "block",
    height: "81vh",
  };
  const [toastIsClosed, setToastIsClosed] = useState(false);
  return (
    <Offcanvas
      className="h-100 minScalableWidth"
      show={showOffcanvas}
      onHide={hideFunction}
      placement="bottom"
    >
      <Container
        className="h-100 w-100 p-0"
        onMouseMove={() => {
          if (toastIsClosed) {
            return;
          }
          if (state.complete && !state.error) {
            setIsLoading(false);
            setShowSuccessToast(true);
            let form = document.getElementById("addItemForm");
            form.reset();
            setEans([]);
          }
          if (state.complete && state.error) {
            setIsLoading(false);
            setShowErrorToast(true);
          }
        }}
        fluid
      >
        <Offcanvas.Header className="border-bottom-grey px-xl-5">
          <Container className="px-3" fluid>
            <Row>
              <Col xs="6" lg="9" xl="10" className="d-flex align-items-center">
                <p className="blue-main-text h4 mb-0">Id: {item.itemId}</p>
              </Col>
              <Col xs="4" lg="2" xl="1" className="ps-1 text-end">
                <Button
                  variant="as-link"
                  onClick={() => {
                    setIsProductView(!isProductView);
                  }}
                  className="ps-0"
                >
                  <Image
                    src={
                      isProductView ? switch_product_view : switch_binding_view
                    }
                    alt="switch view"
                  />
                </Button>
              </Col>
              <Col xs="2" lg="1" className="ps-1 text-end">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                  }}
                  className="ps-2"
                >
                  <Image src={CloseIcon} alt="Hide" />
                </Button>
              </Col>
            </Row>
          </Container>
        </Offcanvas.Header>
        <Offcanvas.Body className="px-4 px-xl-5 mx-1 mx-xl-3 pb-0" as="div">
          <Container
            className="p-0"
            style={isProductView ? unhidden : hidden}
            fluid
          >
            <Form action={formAction} id="modifyItemForm">
              <Form.Group
                className="mb-3 maxInputWidth"
                controlId="formPartNumber"
              >
                <Form.Label className="blue-main-text">P/N:</Form.Label>
                <Form.Control
                  className="input-style shadow-sm"
                  type="text"
                  name="partNumber"
                  defaultValue={item.partNumber}
                  maxLength={150}
                />
              </Form.Group>
              <Form.Group className="mb-3 maxInputWidth" controlId="formName">
                <Form.Label className="blue-main-text">Name:</Form.Label>
                <Form.Control
                  className="input-style shadow-sm"
                  type="text"
                  name="name"
                  defaultValue={item.itemName}
                  maxLength={250}
                />
              </Form.Group>
              <Form.Group className="mb-3" controlId="formEans">
                <Stack key={rerenderVar}>
                  <Form.Label className="blue-main-text">EANs:</Form.Label>
                  {eans.map((value, key) => {
                    return (
                      <SpecialInput
                        value={value}
                        key={value}
                        deleteValue={() => {
                          eans.splice(key, 1);
                          if (rerenderVar === 1) {
                            setRerenderVar(rerenderVar + 1);
                          }
                          if (rerenderVar > 1) {
                            setRerenderVar(rerenderVar - 1);
                          }
                        }}
                        modifyValue={(variable) => (eans[key] = variable)}
                        eanExistFun={eanExist}
                      />
                    );
                  })}
                  <Button
                    variant="mainBlue"
                    className="mb-3 mt-4 ms-3 py-3"
                    style={buttonStyle}
                    onClick={() => setIsAddEanShow(true)}
                  >
                    Add Ean
                  </Button>
                  <AddEanWindow
                    modalShow={isAddEanShow}
                    onHideFunction={() => setIsAddEanShow(false)}
                    addAction={(variable) => eans.push(variable)}
                    eanExistFun={eanExist}
                  />
                </Stack>
              </Form.Group>
              <Form.Group className="mb-5" controlId="formDescription">
                <Form.Label className="blue-main-text maxInputWidth-desc">
                  Description:
                </Form.Label>
                <Form.Control
                  className="input-style shadow-sm"
                  as="textarea"
                  rows={5}
                  type="text"
                  name="description"
                  defaultValue={
                    item.itemDescription ? item.itemDescription : "Loading.."
                  }
                  maxLength={500}
                />
              </Form.Group>
            </Form>
          </Container>
          <Container
            className="p-0"
            style={!isProductView ? unhidden : hidden}
            fluid
          >
            <Form>
              <Form.Group className="mb-5" controlId="formDescription">
                <Form.Label className="blue-main-text maxInputWidth">
                  Users: {bindings.username}
                </Form.Label>
                {bindings.map((value) => {
                  return <BindingInput value={value} key={value} />;
                })}
              </Form.Group>
              <Button
                variant="mainBlue"
                className="mb-3 mt-2 ms-3 py-3 w-100"
                style={buttonStyle}
                type="sumbit"
              >
                Save
              </Button>
            </Form>
          </Container>
          <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
            <Row style={maxStyle} className="mx-auto minScalableWidth">
              <Col>
                <Button
                  variant="mainBlue"
                  className="w-100"
                  onClick={(e) => {
                    e.preventDefault();
                    setIsLoading(true);
                    setToastIsClosed(false);

                    let form = document.getElementById("modifyItemForm");
                    form.requestSubmit();
                  }}
                >
                  {isLoading && !state.complete ? (
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
                    setEans([]);
                    hideFunction();
                    if (!state.error && state.complete) {
                      router.refresh();
                    }
                  }}
                >
                  Cancel
                </Button>
              </Col>
            </Row>
          </Container>
        </Offcanvas.Body>

        <Toastes.ErrorToast
          showToast={showErrorToast}
          message="Could not create item"
          onHideFun={() => {
            setToastIsClosed(true);
            setShowErrorToast(false);
          }}
        />
        <Toastes.SuccessToast
          showToast={showSuccessToast}
          message="Item succesfuly created."
          onHideFun={() => {
            setToastIsClosed(true);
            setShowSuccessToast(false);
          }}
        />
      </Container>
    </Offcanvas>
  );
}

ModifyItemOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
};

export default ModifyItemOffcanvas;
