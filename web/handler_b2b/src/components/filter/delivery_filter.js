import Image from "next/image";
import PropTypes from "prop-types";
import {
  Offcanvas,
  Container,
  Row,
  Col,
  Button,
  Stack,
  Form,
  InputGroup,
} from "react-bootstrap";
import CloseIcon from "../../../public/icons/close_black.png";
import { useEffect, useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import ErrorMessage from "../smaller_components/error_message";
import getOrgsList from "@/utils/documents/get_orgs_list";
import getDeliveryCompany from "@/utils/deliveries/get_delivery_company";
import getDeliveryStatuses from "@/utils/deliveries/get_delivery_statuses";

function DeliveryFilterOffcanvas({
  showOffcanvas,
  hideFunction,
  currentSort,
  currentDirection,
}) {
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  const newParams = new URLSearchParams(params);
  const [isAsc, setIsAsc] = useState(currentDirection);
  const orderBy = ["Id", "Estimated", "Recipient", "Proforma_Number"];
  const [orgs, setOrgs] = useState([]);
  const [deliveries, setDeliveries] = useState([]);
  const [statuses, setStatuses] = useState([]);
  const [errorDownloadOrgs, setDownloadErrorOrgs] = useState(false);
  const [errorDownloadDeli, setDownloadErrorDeli] = useState(false);
  const [errorDownloadStatus, setDownloadErrorStatus] = useState(false);
  useEffect(() => {
    getOrgsList().then((data) => {
      if (data !== null) {
        setDownloadErrorOrgs(false);
        setOrgs(data.restOrgs);
      } else {
        setDownloadErrorOrgs(true);
      }
    });
    getDeliveryCompany().then((data) => {
      if (data === null) {
        setDownloadErrorDeli(true);
      } else {
        setDeliveries(data);
        setDownloadErrorDeli(false);
      }
    });
    getDeliveryStatuses().then((data) => {
      if (data === null) {
        setDownloadErrorStatus(true);
      } else {
        setStatuses(data);
        setDownloadErrorStatus(false);
      }
    });
  }, []);
  // Styles
  const vhStyle = {
    height: "81vh",
  };
  const maxStyle = {
    maxWidth: "393px",
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
                <p className="blue-main-text h4 mb-0">Filter/Sort by</p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
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
          <Container className="p-0 mx-1 mx-xl-3" style={vhStyle} fluid>
            <ErrorMessage
              message="Could not download recipients."
              messageStatus={errorDownloadOrgs}
            />
            <ErrorMessage
              message="Could not download statuses."
              messageStatus={errorDownloadStatus}
            />
            <ErrorMessage
              message="Could not download companies."
              messageStatus={errorDownloadDeli}
            />
            <Container className="px-1 ms-0 pb-3">
              <p className="mb-1 blue-main-text">Sort order</p>
              <Stack
                direction="horizontal"
                className="align-items-center"
                style={{ maxWidth: "329px" }}
              >
                <Button
                  className="w-100 me-2"
                  disabled={isAsc}
                  onClick={() => setIsAsc(true)}
                >
                  Ascending
                </Button>
                <Button
                  className="w-100 ms-2"
                  variant="red"
                  disabled={!isAsc}
                  onClick={() => setIsAsc(false)}
                >
                  Descending
                </Button>
              </Stack>
            </Container>
            <Container className="px-1 ms-0 mb-3">
              <p className="blue-main-text">Sort:</p>
              <Form.Select
                className="input-style"
                id="sortValue"
                style={maxStyle}
                defaultValue={currentSort.substring(1, currentSort.lenght)}
              >
                <option value="None">None</option>
                {Object.values(orderBy).map((val) => {
                  return (
                    <option value={val} key={val}>
                      {val}
                    </option>
                  );
                })}
              </Form.Select>
            </Container>
            <Container className="px-1 ms-0 pb-5">
              <p className="mb-3 blue-main-text">Filters:</p>
              <Stack className="mb-2">
                <p className="mb-1 blue-sec-text">Estimated:</p>
                <Container className="px-0">
                  <Row className="gy-2">
                    <Col xs="12" md="4" lg="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {"<="}
                        </InputGroup.Text>
                        <Form.Control
                          type="date"
                          id="estimatedL"
                          defaultValue={newParams.get("estimatedL") ?? ""}
                          className="main-grey-bg"
                        />
                      </InputGroup>
                    </Col>
                    <Col xs="12" md="4" lg="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {">="}
                        </InputGroup.Text>
                        <Form.Control
                          type="date"
                          id="estimatedG"
                          defaultValue={newParams.get("estimatedG") ?? ""}
                          className="main-grey-bg"
                        />
                      </InputGroup>
                    </Col>
                  </Row>
                </Container>
              </Stack>
              <Stack className="mb-2">
                <p className="mb-1 blue-sec-text">Delivered:</p>
                <Container className="px-0">
                  <Row className="gy-2">
                    <Col xs="12" md="4" lg="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {"<="}
                        </InputGroup.Text>
                        <Form.Control
                          type="date"
                          id="deliveredL"
                          defaultValue={newParams.get("deliveredL") ?? ""}
                          className="main-grey-bg"
                        />
                      </InputGroup>
                    </Col>
                    <Col xs="12" md="4" lg="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {">="}
                        </InputGroup.Text>
                        <Form.Control
                          type="date"
                          id="deliveredG"
                          defaultValue={newParams.get("deliveredG") ?? ""}
                          className="main-grey-bg"
                        />
                      </InputGroup>
                    </Col>
                  </Row>
                </Container>
              </Stack>
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Recipient:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="recipient"
                    defaultValue={newParams.get("recipient") ?? "none"}
                  >
                    <option value="none">None</option>
                    {orgs.map((value) => {
                      return (
                        <option key={value.orgId} value={value.orgId}>
                          {value.orgName}
                        </option>
                      );
                    })}
                  </Form.Select>
                </Container>
              </Stack>
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Status:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="status"
                    defaultValue={newParams.get("status") ?? "none"}
                  >
                    <option value="none">None</option>
                    {statuses.map((value) => {
                      return (
                        <option key={value.id} value={value.id}>
                          {value.name}
                        </option>
                      );
                    })}
                  </Form.Select>
                </Container>
              </Stack>
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Delivery company:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="company"
                    defaultValue={newParams.get("company") ?? "none"}
                  >
                    <option value="none">None</option>
                    {deliveries.map((value) => {
                      return (
                        <option key={value.id} value={value.id}>
                          {value.name}
                        </option>
                      );
                    })}
                  </Form.Select>
                </Container>
              </Stack>
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Waybill:</p>
                <Container className="px-0">
                  <InputGroup style={maxStyle}>
                    <Form.Control
                      type="text"
                      className="input-style"
                      id="waybill"
                      defaultValue={newParams.get("waybill") ?? ""}
                    />
                    <InputGroup.Text className="main-blue-bg main-text">
                      {"Contains"}
                    </InputGroup.Text>
                  </InputGroup>
                </Container>
              </Stack>
            </Container>
          </Container>
          <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
            <Row style={maxStyle} className="mx-auto minScalableWidth">
              <Col>
                <Button
                  variant="green"
                  className="w-100"
                  onClick={() => {
                    let status = document.getElementById("status").value;
                    if (status !== "none") newParams.set("status", status);
                    if (status === "none") newParams.delete("status");

                    let recipient = document.getElementById("recipient").value;
                    if (recipient !== "none")
                      newParams.set("recipient", recipient);
                    if (recipient === "none") newParams.delete("recipient");

                    let company = document.getElementById("company").value;
                    if (company !== "none") newParams.set("company", company);
                    if (company === "none") newParams.delete("company");

                    let estimatedL =
                      document.getElementById("estimatedL").value;
                    if (estimatedL) newParams.set("estimatedL", estimatedL);
                    if (!estimatedL) newParams.delete("estimatedL");

                    let estimatedG =
                      document.getElementById("estimatedG").value;
                    if (estimatedG) newParams.set("estimatedG", estimatedG);
                    if (!estimatedG) newParams.delete("estimatedG");

                    let deliveredL =
                      document.getElementById("deliveredL").value;
                    if (deliveredL) newParams.set("deliveredL", deliveredL);
                    if (!deliveredL) newParams.delete("deliveredL");

                    let deliveredG =
                      document.getElementById("deliveredG").value;
                    if (deliveredG) newParams.set("deliveredG", deliveredG);
                    if (!deliveredG) newParams.delete("deliveredG");

                    let waybill = document.getElementById("waybill").value;
                    if (waybill) newParams.set("waybill", waybill);
                    if (!waybill) newParams.delete("waybill");

                    let sort = document.getElementById("sortValue").value;
                    if (sort != "None") {
                      sort = isAsc ? "A" + sort : "D" + sort;
                      newParams.set("orderBy", sort);
                    } else {
                      newParams.delete("orderBy");
                    }
                    router.replace(`${pathName}?${newParams}`);
                    hideFunction();
                  }}
                >
                  Save
                </Button>
              </Col>
              <Col>
                <Button
                  variant="mainBlue"
                  className="w-100"
                  onClick={() => {
                    newParams.delete("orderBy");
                    newParams.delete("status");
                    newParams.delete("recipient");
                    newParams.delete("company");
                    newParams.delete("estimatedL");
                    newParams.delete("estimatedG");
                    newParams.delete("deliveredL");
                    newParams.delete("deliveredG");
                    newParams.delete("waybill");
                    router.replace(`${pathName}?${newParams}`);
                    hideFunction();
                  }}
                >
                  Clear all
                </Button>
              </Col>
            </Row>
          </Container>
        </Offcanvas.Body>
      </Container>
    </Offcanvas>
  );
}

DeliveryFilterOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
};

export default DeliveryFilterOffcanvas;
