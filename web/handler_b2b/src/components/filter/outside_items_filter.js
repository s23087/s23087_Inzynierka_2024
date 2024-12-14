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
import FilterHeader from "./filter_header";
import SetQueryFunc from "./filters_query_functions";
import SortOrderComponent from "./sort_component";

/**
 * Create offcanvas that allow to filter and sort outside item objects.
 * @component
 * @param {object} props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {string} props.currentSort Current sort value
 * @param {boolean} props.currentDirection True if ascending, false if descending.
 * @return {JSX.Element} Offcanvas element
 */
function OutsideItemsFilterOffcanvas({
  showOffcanvas,
  hideFunction,
  currentSort,
  currentDirection,
}) {
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  const newParams = new URLSearchParams(params);
  // Download data holder
  const [orgs, setOrgs] = useState([]);
  // True if download error happen
  const [errorDownload, setDownloadError] = useState(false);
  // download data
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
  // True is ascending order is enabled
  const [isAsc, setIsAsc] = useState(currentDirection);
  // Order options
  const orderBy = ["Id", "Source", "Qty", "Price", "Partnumber"];
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
              messageStatus={errorDownload}
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
              <Stack className="mb-2">
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
              <Stack className="mb-2">
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
                <p className="mb-1 blue-sec-text">Source:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="source"
                    defaultValue={newParams.get("source") ?? "none"}
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
                <Container className="px-0 pb-5">
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
            <Row style={maxStyle} className="mx-auto minScalableWidth offcanvasButtonsStyle">
              <Col>
                <Button
                  variant="green"
                  className="w-100"
                  onClick={() => {
                    SetQueryFunc.setCurrencyFilter(newParams);
                    SetQueryFunc.setSourceFilter(newParams);
                    SetQueryFunc.setPriceFilter(newParams);
                    SetQueryFunc.setQtyFilter(newParams);

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
                    newParams.delete("priceL");
                    newParams.delete("priceG");
                    newParams.delete("qtyL");
                    newParams.delete("qtyG");
                    newParams.delete("source");
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

OutsideItemsFilterOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
};

export default OutsideItemsFilterOffcanvas;
