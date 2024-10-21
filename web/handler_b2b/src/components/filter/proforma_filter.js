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
import { useEffect, useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import validators from "@/utils/validators/validator";
import ErrorMessage from "../smaller_components/error_message";
import getOrgsList from "@/utils/documents/get_orgs_list";
import FilterHeader from "./filter_header";

function ProformaFilterOffcanvas({
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
  const orderBy = ["Number", "Date", "Recipient", "Qty", "Total"];
  const [orgs, setOrgs] = useState([]);
  const [errorDownload, setDownloadError] = useState(false);
  useEffect(() => {
    getOrgsList().then((data) => {
      if (data !== null) {
        setDownloadError(false);
        setOrgs(data.restOrgs);
      } else {
        setDownloadError(true);
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
        <FilterHeader 
          hideFunction={hideFunction}
        />
        <Offcanvas.Body className="px-4 px-xl-5 pb-0" as="div">
          <Container className="p-0 mx-1 mx-xl-3" style={vhStyle} fluid>
            <ErrorMessage
              message="Could not download recipients."
              messageStatus={errorDownload}
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
                <p className="mb-1 blue-sec-text">Total products:</p>
                <Container className="px-0">
                  <Row className="gy-2">
                    <Col xs="6" md="2">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {"<="}
                        </InputGroup.Text>
                        <Form.Control
                          type="text"
                          id="qtyL"
                          defaultValue={newParams.get("qtyL") ?? ""}
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
                    <Col xs="6" md="2">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {">="}
                        </InputGroup.Text>
                        <Form.Control
                          type="text"
                          id="qtyG"
                          defaultValue={newParams.get("qtyG") ?? ""}
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
                <p className="mb-1 blue-sec-text">Total value:</p>
                <Container className="px-0">
                  <Row className="gy-2">
                    <Col xs="6" md="2">
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
                    <Col xs="6" md="2">
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
                <p className="mb-1 blue-sec-text">Date:</p>
                <Container className="px-0">
                  <Row className="gy-2">
                    <Col xs="12" md="4" lg="3">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {"<="}
                        </InputGroup.Text>
                        <Form.Control
                          type="date"
                          id="dateL"
                          defaultValue={newParams.get("dateL") ?? ""}
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
                          id="dateG"
                          defaultValue={newParams.get("dateG") ?? ""}
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
            </Container>
          </Container>
          <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
            <Row style={maxStyle} className="mx-auto minScalableWidth">
              <Col>
                <Button
                  variant="green"
                  className="w-100"
                  onClick={() => {
                    setCurrencyFilter();
                    setRecipientFilter();
                    setTotalFilter();
                    setQtyFilter();
                    setDateFilter();

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
                    newParams.delete("totalL");
                    newParams.delete("totalG");
                    newParams.delete("qtyL");
                    newParams.delete("qtyG");
                    newParams.delete("dateL");
                    newParams.delete("dateG");
                    newParams.delete("recipient");
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

  function setDateFilter() {
    let dateL = document.getElementById("dateL").value;
    if (dateL) newParams.set("dateL", dateL);
    if (!dateL) newParams.delete("dateL");
    let dateG = document.getElementById("dateG").value;
    if (dateG) newParams.set("dateG", dateG);
    if (!dateG) newParams.delete("dateG");
  }

  function setQtyFilter() {
    let qtyG = document.getElementById("qtyG").value;
    if (qtyG) newParams.set("qtyG", qtyG);
    if (!qtyG) newParams.delete("qtyG");
    let qtyL = document.getElementById("qtyL").value;
    if (qtyL) newParams.set("qtyL", qtyL);
    if (!qtyL) newParams.delete("qtyL");
  }

  function setTotalFilter() {
    let totalL = document.getElementById("totalL").value;
    if (validators.haveOnlyNumbers(totalL) && totalL)
      newParams.set("totalL", totalL);
    if (!totalL) newParams.delete("totalL");
    let totalG = document.getElementById("totalG").value;
    if (validators.haveOnlyNumbers(totalG) && totalG)
      newParams.set("totalG", totalG);
    if (!totalG) newParams.delete("totalG");
  }

  function setRecipientFilter() {
    let recipient = document.getElementById("recipient").value;
    if (recipient !== "none") newParams.set("recipient", recipient);
    if (recipient === "none") newParams.delete("recipient");
  }

  function setCurrencyFilter() {
    let currencyFilter = document.getElementById("currencyFilter").value;
    if (currencyFilter !== "none") newParams.set("currency", currencyFilter);
    if (currencyFilter === "none") newParams.delete("currency");
  }
}

ProformaFilterOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
};

export default ProformaFilterOffcanvas;
