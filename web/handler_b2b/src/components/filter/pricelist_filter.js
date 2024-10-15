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
import { useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import validators from "@/utils/validators/validator";

function PricelistFilterOffcanvas({
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
  const orderBy = ["Created", "Name", "Modified", "Products"];
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
                <p className="mb-1 blue-sec-text">Total products:</p>
                <Container className="px-0">
                  <Row className="gy-2">
                    <Col xs="6" md="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {"<="}
                        </InputGroup.Text>
                        <Form.Control
                          type="text"
                          id="totalL"
                          defaultValue={newParams.get("totalL") ?? ""}
                          className="main-grey-bg"
                          onInput={(e) =>
                            (e.target.value = e.target.value.replaceAll(
                              /\D/g,
                              "",
                            ))
                          }
                        />
                      </InputGroup>
                    </Col>
                    <Col xs="6" md="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {">="}
                        </InputGroup.Text>
                        <Form.Control
                          type="text"
                          id="totalG"
                          defaultValue={newParams.get("totalG") ?? ""}
                          className="main-grey-bg"
                          onInput={(e) =>
                            (e.target.value = e.target.value.replaceAll(
                              /\D/g,
                              "",
                            ))
                          }
                        />
                      </InputGroup>
                    </Col>
                  </Row>
                </Container>
              </Stack>
              <Stack className="mb-2">
                <p className="mb-1 blue-sec-text">Created:</p>
                <Container className="px-0">
                  <Row className="gy-2">
                    <Col xs="12" md="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {"<="}
                        </InputGroup.Text>
                        <Form.Control
                          type="date"
                          id="createdL"
                          defaultValue={newParams.get("createdL") ?? ""}
                          className="main-grey-bg"
                        />
                      </InputGroup>
                    </Col>
                    <Col xs="12" md="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {">="}
                        </InputGroup.Text>
                        <Form.Control
                          type="date"
                          id="createdG"
                          defaultValue={newParams.get("createdG") ?? ""}
                          className="main-grey-bg"
                        />
                      </InputGroup>
                    </Col>
                  </Row>
                </Container>
              </Stack>
              <Stack className="mb-2">
                <p className="mb-1 blue-sec-text">Modified:</p>
                <Container className="px-0">
                  <Row className="gy-2">
                    <Col xs="12" md="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {"<="}
                        </InputGroup.Text>
                        <Form.Control
                          type="date"
                          id="modifiedL"
                          defaultValue={newParams.get("modifiedL") ?? ""}
                          className="main-grey-bg"
                        />
                      </InputGroup>
                    </Col>
                    <Col xs="12" md="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {">="}
                        </InputGroup.Text>
                        <Form.Control
                          type="date"
                          id="modifiedG"
                          defaultValue={newParams.get("modifiedG") ?? ""}
                          className="main-grey-bg"
                        />
                      </InputGroup>
                    </Col>
                  </Row>
                </Container>
              </Stack>
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Status:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="filterStatus"
                    defaultValue={newParams.get("status") ?? "none"}
                  >
                    <option value="none">None</option>
                    <option value="Active">Active</option>
                    <option value="Deactivated">Deactivated</option>
                  </Form.Select>
                </Container>
              </Stack>
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Currency:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="currencyFilter"
                    defaultValue={newParams.get("currency") ?? "none"}
                  >
                    <option value="none">None</option>
                    <option value="PLN">PLN</option>
                    <option value="USD">USD</option>
                    <option value="EUR">EUR</option>
                  </Form.Select>
                </Container>
              </Stack>
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Type:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="typeFilter"
                    defaultValue={newParams.get("typeFilter") ?? "none"}
                  >
                    <option value="none">None</option>
                    <option value="csv">CSV</option>
                    <option value="xlsx">XML</option>
                  </Form.Select>
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
                    let statusFilter =
                      document.getElementById("filterStatus").value;
                    if (statusFilter !== "none")
                      newParams.set("status", statusFilter);
                    if (statusFilter === "none") newParams.delete("status");
                    let currencyFilter =
                      document.getElementById("currencyFilter").value;
                    if (currencyFilter !== "none")
                      newParams.set("currency", currencyFilter);
                    if (currencyFilter === "none") newParams.delete("currency");
                    let typeFilter =
                      document.getElementById("typeFilter").value;
                    if (typeFilter !== "none")
                      newParams.set("type", typeFilter);
                    if (typeFilter === "none") newParams.delete("type");
                    let totalL = document.getElementById("totalL").value;
                    if (validators.haveOnlyNumbers(totalL) && totalL)
                      newParams.set("totalL", totalL);
                    if (!totalL) newParams.delete("totalL");
                    let totalG = document.getElementById("totalG").value;
                    if (validators.haveOnlyNumbers(totalG) && totalG)
                      newParams.set("totalG", totalG);
                    if (!totalG) newParams.delete("totalG");
                    let createdG = document.getElementById("createdG").value;
                    if (createdG) newParams.set("createdG", createdG);
                    if (!createdG) newParams.delete("createdG");
                    let createdL = document.getElementById("createdL").value;
                    if (createdL) newParams.set("createdL", createdL);
                    if (!createdL) newParams.delete("createdL");
                    let modifiedL = document.getElementById("modifiedL").value;
                    if (modifiedL) newParams.set("modifiedL", modifiedL);
                    if (!modifiedL) newParams.delete("modifiedL");
                    let modifiedG = document.getElementById("modifiedG").value;
                    if (modifiedG) newParams.set("modifiedG", modifiedG);
                    if (!modifiedG) newParams.delete("modifiedG");

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
                    newParams.delete("totalL");
                    newParams.delete("totalG");
                    newParams.delete("createdL");
                    newParams.delete("createdG");
                    newParams.delete("modifiedL");
                    newParams.delete("modifiedG");
                    newParams.delete("type");
                    newParams.delete("currency");
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

PricelistFilterOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
};

export default PricelistFilterOffcanvas;
