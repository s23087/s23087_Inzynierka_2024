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
import { useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import FilterHeader from "./filter_header";
import SetQueryFunc from "./filters_query_functions";
import SortOrderComponent from "./sort_component";

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
        <FilterHeader 
          hideFunction={hideFunction}
        />
        <Offcanvas.Body className="px-4 px-xl-5 pb-0" as="div">
          <Container className="p-0 mx-1 mx-xl-3" style={vhStyle} fluid>
          <SortOrderComponent 
              isAsc={isAsc}
              setIsAsc={setIsAsc}
            />
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
                    SetQueryFunc.setStatusFilter(newParams);
                    SetQueryFunc.setCurrencyFilter(newParams);
                    SetQueryFunc.setTypeFilter(newParams);
                    SetQueryFunc.setTotalFilter(newParams);
                    SetQueryFunc.setCreatedFilter(newParams);
                    SetQueryFunc.setModifiedFilter(newParams);

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

PricelistFilterOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
};

export default PricelistFilterOffcanvas;
