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
import validators from "@/utils/validators/validator";
import FilterHeader from "./filter_header";

function ProductFilterOffcanvas({
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
  const orderBy = ["Id", "Partnumber", "Qty", "Price", "Name"];
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
            <Container className="px-1 ms-0">
              <p className="mb-3 blue-main-text">Filters:</p>
              <Stack>
                <p className="mb-1 blue-sec-text">Qty:</p>
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
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Price:</p>
                <Container className="px-0">
                  <Row className="gy-2">
                    <Col xs="6" md="2">
                      <InputGroup>
                        <InputGroup.Text className="main-blue-bg main-text">
                          {"<="}
                        </InputGroup.Text>
                        <Form.Control
                          type="text"
                          id="priceL"
                          defaultValue={newParams.get("priceL") ?? ""}
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
                          id="priceG"
                          defaultValue={newParams.get("priceG") ?? ""}
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
                <p className="mb-1 blue-sec-text">Status:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="filterStatus"
                    defaultValue={newParams.get("status") ?? "none"}
                  >
                    <option value="none">None</option>
                    <option value="war">In warehouse</option>
                    <option value="deli">In delivery</option>
                    <option value="wardeli">In warehouse | In delivery</option>
                    <option value="req">On request</option>
                    <option value="unavail">Unavailable</option>
                  </Form.Select>
                </Container>
              </Stack>
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Ean:</p>
                <Container className="px-0">
                  <InputGroup style={maxStyle}>
                    <Form.Control
                      type="text"
                      className="input-style"
                      id="eanFilter"
                      defaultValue={newParams.get("ean") ?? ""}
                      onInput={(e) =>
                        (e.target.value = e.target.value.replaceAll(/\D/g, ""))
                      }
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
                    setStatusFilter();
                    setEanFilter();
                    setQtyFilter();
                    setPriceFilter();

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
                    newParams.delete("ean");
                    newParams.delete("qtyL");
                    newParams.delete("qtyG");
                    newParams.delete("priceG");
                    newParams.delete("priceL");
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

  function setPriceFilter() {
    let priceLess = document.getElementById("priceL").value;
    if (validators.haveOnlyNumbers(priceLess) && priceLess)
      newParams.set("priceL", priceLess);
    if (!priceLess) newParams.delete("priceL");
    let priceGreater = document.getElementById("priceG").value;
    if (validators.haveOnlyNumbers(priceGreater) && priceGreater)
      newParams.set("priceG", priceGreater);
    if (!priceGreater) newParams.delete("priceG");
  }

  function setQtyFilter() {
    let qtyGreater = document.getElementById("qtyG").value;
    if (validators.haveOnlyNumbers(qtyGreater) && qtyGreater)
      newParams.set("qtyG", qtyGreater);
    if (!qtyGreater) newParams.delete("qtyG");
    let qtyLess = document.getElementById("qtyL").value;
    if (validators.haveOnlyNumbers(qtyLess) && qtyLess)
      newParams.set("qtyL", qtyLess);
    if (!qtyLess) newParams.delete("qtyL");
  }

  function setEanFilter() {
    let eanFilter = document.getElementById("eanFilter").value;
    if (validators.haveOnlyNumbers(eanFilter) && eanFilter)
      newParams.set("ean", eanFilter);
    if (!eanFilter) newParams.delete("ean");
  }

  function setStatusFilter() {
    let statusFilter = document.getElementById("filterStatus").value;
    if (statusFilter !== "none") newParams.set("status", statusFilter);
    if (statusFilter === "none") newParams.delete("status");
  }
}

ProductFilterOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
};

export default ProductFilterOffcanvas;
