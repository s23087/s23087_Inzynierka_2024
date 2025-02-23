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
import ErrorMessage from "../smaller_components/error_message";
import getOrgsList from "@/utils/documents/get_orgs_list";
import getPaymentStatuses from "@/utils/documents/get_payment_statuses";
import getRequestStatuses from "@/utils/documents/get_request_statuses";
import FilterHeader from "./filter_header";
import SetQueryFunc from "./filters_query_functions";
import SortOrderComponent from "./sort_component";

/**
 * Depend on document type return sort option
 * @param {string} type document current type
 * @return {Array<string>} Return sort option of chosen type
 */
function getOrderBy(type) {
  if (type.includes("invoice"))
    return ["Number", "Date", "Qty", "Total", "Due"];
  if (type.includes("note")) return ["Number", "Date", "Qty", "Total"];
  return ["Title", "Date"];
}

/**
 * Create offcanvas that allow to filter and sort document objects.
 * @component
 * @param {object} props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {string} props.currentSort Current sort value
 * @param {boolean} props.currentDirection True if ascending, false if descending.
 * @param {string} props.type Document type name
 * @return {JSX.Element} Offcanvas element
 */
function InvoiceFilterOffcanvas({
  showOffcanvas,
  hideFunction,
  currentSort,
  currentDirection,
  type,
}) {
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  const newParams = new URLSearchParams(params);
  // True is ascending order is enabled
  const [isAsc, setIsAsc] = useState(currentDirection);
  // Order options
  const orderBy = getOrderBy(type);
  // download data holder
  const [orgs, setOrgs] = useState([]);
  const [paymentStatuses, setPaymentStatuses] = useState([]);
  const [requestStatuses, setRequestStatuses] = useState([]);
  // download errors
  const [errorDownloadOrg, setDownloadErrorOrg] = useState(false);
  const [errorDownloadPay, setDownloadErrorPay] = useState(false);
  const [errorDownloadReq, setDownloadErrorReq] = useState(false);
  // download data
  useEffect(() => {
    if (!type.includes("Request")) {
      getOrgsList().then((data) => {
        if (data !== null) {
          setDownloadErrorOrg(false);
          setOrgs(data.restOrgs);
        } else {
          setDownloadErrorOrg(true);
        }
      });

      if (type.includes("invoice")) {
        getPaymentStatuses().then((data) => {
          if (data !== null) {
            setDownloadErrorPay(false);
            setPaymentStatuses(data);
          } else {
            setDownloadErrorPay(true);
          }
        });
      }
    } else {
      getRequestStatuses().then((data) => {
        if (data !== null) {
          setDownloadErrorReq(false);
          setRequestStatuses(data);
        } else {
          setDownloadErrorReq(true);
        }
      });
    }
  }, [type]);
  // Styles
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
        <FilterHeader hideFunction={hideFunction} />
        <Offcanvas.Body className="px-4 px-xl-5 pb-0 scrollableHeight" as="div">
          <Container className="p-0 mx-1 mx-xl-3" fluid>
            <ErrorMessage
              message="Could not download recipients."
              messageStatus={errorDownloadOrg}
            />
            <ErrorMessage
              message="Could not download payment statuses."
              messageStatus={errorDownloadPay}
            />
            <ErrorMessage
              message="Could not download request statuses."
              messageStatus={errorDownloadReq}
            />
            <SortOrderComponent isAsc={isAsc} setIsAsc={setIsAsc} />
            <Container className="px-1 ms-0 mb-3">
              <p className="blue-main-text">Sort:</p>
              <Form.Select
                className="input-style"
                id="sortValue"
                style={maxStyle}
                defaultValue={currentSort.substring(1, currentSort.length)}
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
              {type.includes("invoice") ? (
                <>
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
                  <Stack className="mb-2">
                    <p className="mb-1 blue-sec-text">Due date:</p>
                    <Container className="px-0">
                      <Row className="gy-2">
                        <Col xs="12" md="4" lg="3">
                          <InputGroup>
                            <InputGroup.Text className="main-blue-bg main-text">
                              {"<="}
                            </InputGroup.Text>
                            <Form.Control
                              type="date"
                              id="dueL"
                              defaultValue={newParams.get("dueL") ?? ""}
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
                              id="dueG"
                              defaultValue={newParams.get("dueG") ?? ""}
                              className="main-grey-bg"
                            />
                          </InputGroup>
                        </Col>
                      </Row>
                    </Container>
                  </Stack>
                  <Stack className="mb-2">
                    <p className="mb-1 blue-sec-text">Qty:</p>
                    <Container className="px-0">
                      <Row className="gy-2">
                        <Col xs="6" md="3">
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
                        <Col xs="6" md="3">
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
                    <p className="mb-1 blue-sec-text">Total price:</p>
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
                  <Stack className="mt-2">
                    <p className="mb-1 blue-sec-text">Payment status:</p>
                    <Container className="px-0">
                      <Form.Select
                        className="input-style"
                        style={maxStyle}
                        id="paymentStatus"
                        defaultValue={newParams.get("paymentStatus") ?? "none"}
                      >
                        <option value="none">None</option>
                        {paymentStatuses.map((value) => {
                          return (
                            <option
                              key={value.paymentStatusId}
                              value={value.paymentStatusId}
                            >
                              {value.statusName}
                            </option>
                          );
                        })}
                      </Form.Select>
                    </Container>
                  </Stack>
                  <Stack className="mt-2 mb-5">
                    <p className="mb-1 blue-sec-text">Status:</p>
                    <Container className="px-0">
                      <Form.Select
                        className="input-style"
                        style={maxStyle}
                        id="filterStatus"
                        defaultValue={newParams.get("status") ?? "none"}
                      >
                        <option value="none">None</option>
                        <option value={true}>In system</option>
                        <option value={false}>Not in system</option>
                      </Form.Select>
                    </Container>
                  </Stack>
                </>
              ) : null}
              {type.includes("notes") ? (
                <>
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
                  <Stack className="mb-2">
                    <p className="mb-1 blue-sec-text">Qty:</p>
                    <Container className="px-0">
                      <Row className="gy-2">
                        <Col xs="6" md="3">
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
                        <Col xs="6" md="3">
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
                  <Stack className="mt-2">
                    <p className="mb-1 blue-sec-text">Payment status:</p>
                    <Container className="px-0">
                      <Form.Select
                        className="input-style"
                        style={maxStyle}
                        id="paymentStatus"
                        defaultValue={newParams.get("paymentStatus") ?? "none"}
                      >
                        <option value="none">None</option>
                        <option value={true}>Paid</option>
                        <option value={false}>Unpaid</option>
                      </Form.Select>
                    </Container>
                  </Stack>
                  <Stack className="mt-2 mb-5">
                    <p className="mb-1 blue-sec-text">Status:</p>
                    <Container className="px-0">
                      <Form.Select
                        className="input-style"
                        style={maxStyle}
                        id="filterStatus"
                        defaultValue={newParams.get("status") ?? "none"}
                      >
                        <option value="none">None</option>
                        <option value={true}>In system</option>
                        <option value={false}>Not in system</option>
                      </Form.Select>
                    </Container>
                  </Stack>
                </>
              ) : null}

              {type.includes("Request") ? (
                <>
                  <Stack className="mb-2">
                    <p className="mb-1 blue-sec-text">Date:</p>
                    <Container className="px-0">
                      <Row className="gy-2">
                        <Col xs="12" md="3">
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
                        <Col xs="12" md="3">
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
                    <p className="mb-1 blue-sec-text">Type:</p>
                    <Container className="px-0">
                      <Form.Select
                        className="input-style"
                        style={maxStyle}
                        id="typeFilter"
                        defaultValue={newParams.get("type") ?? "none"}
                      >
                        <option value="none">None</option>
                        <option value="Sales invoices">Sales invoices</option>
                        <option value="Yours invoices">Yours invoices</option>
                        <option value="Yours credit notes">
                          Yours credit notes
                        </option>
                        <option value="Client credit notes">
                          Client credit notes
                        </option>
                        <option value="Yours Proformas">Yours Proformas</option>
                        <option value="Clients Proformas">
                          Clients Proformas
                        </option>
                      </Form.Select>
                    </Container>
                  </Stack>
                  <Stack className="mt-2">
                    <p className="mb-1 blue-sec-text">Status:</p>
                    <Container className="px-0 pb-5">
                      <Form.Select
                        className="input-style"
                        style={maxStyle}
                        id="requestStatus"
                        defaultValue={newParams.get("requestStatus") ?? "none"}
                      >
                        <option value="none">None</option>
                        {requestStatuses.map((value) => {
                          return (
                            <option key={value.id} value={value.id}>
                              {value.name}
                            </option>
                          );
                        })}
                      </Form.Select>
                    </Container>
                  </Stack>
                </>
              ) : null}
            </Container>
          </Container>
          <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
            <Row
              style={maxStyle}
              className="mx-auto minScalableWidth offcanvasButtonsStyle"
            >
              <Col>
                <Button
                  variant="green"
                  className="w-100"
                  disabled={
                    errorDownloadOrg || errorDownloadPay || errorDownloadReq
                  }
                  onClick={() => {
                    if (type.includes("Requests")) {
                      SetQueryFunc.setRequestStatusFilter(newParams);
                      SetQueryFunc.setTypeFilter(newParams);
                    } else {
                      SetQueryFunc.setCurrencyFilter(newParams);
                      SetQueryFunc.setRecipientFilter(newParams);
                      SetQueryFunc.setStatusFilter(newParams);
                      SetQueryFunc.setPaymentStatusFilter(newParams);

                      if (type.includes("invoice")) {
                        SetQueryFunc.setDueFilter(newParams);
                      }

                      SetQueryFunc.setTotalFilter(newParams);
                      SetQueryFunc.setQtyFilter(newParams);
                    }

                    SetQueryFunc.setDateFilter(newParams);

                    SetQueryFunc.setSortFilter(newParams, isAsc);
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
                    newParams.delete("dueL");
                    newParams.delete("dueG");
                    newParams.delete("recipient");
                    newParams.delete("currency");
                    newParams.delete("status");
                    newParams.delete("paymentStatus");
                    newParams.delete("type");
                    newParams.delete("requestStatus");
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

InvoiceFilterOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
  type: PropTypes.string.isRequired,
};

export default InvoiceFilterOffcanvas;
