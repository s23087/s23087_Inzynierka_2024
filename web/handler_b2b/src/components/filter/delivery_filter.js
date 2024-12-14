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
import getDeliveryCompany from "@/utils/deliveries/get_delivery_company";
import getDeliveryStatuses from "@/utils/deliveries/get_delivery_statuses";
import FilterHeader from "./filter_header";
import SetQueryFunc from "./filters_query_functions";
import SortOrderComponent from "./sort_component";

/**
 * Create offcanvas that allow to filter and sort delivery objects.
 * @component
 * @param {object} props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {string} props.currentSort Current sort value
 * @param {boolean} props.currentDirection True if ascending, false if descending.
 * @return {JSX.Element} Offcanvas element
 */
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
  // True is ascending order is enabled
  const [isAsc, setIsAsc] = useState(currentDirection);
  // Order options
  const orderBy = ["Id", "Estimated", "Recipient", "Proforma_Number"];
  // download holder
  const [orgs, setOrgs] = useState([]);
  const [deliveries, setDeliveries] = useState([]);
  const [statuses, setStatuses] = useState([]);
  // download error
  const [errorDownloadOrgs, setDownloadErrorOrgs] = useState(false);
  const [errorDownloadDeli, setDownloadErrorDeli] = useState(false);
  const [errorDownloadStatus, setDownloadErrorStatus] = useState(false);
  // download data
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
          <Container className="p-0 pb-5 mx-1 mx-xl-3" fluid>
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
                    id="filterStatus"
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
            <Row style={maxStyle} className="mx-auto minScalableWidth offcanvasButtonsStyle">
              <Col>
                <Button
                  variant="green"
                  className="w-100"
                  onClick={() => {
                    SetQueryFunc.setStatusFilter(newParams);
                    SetQueryFunc.setRecipientFilter(newParams);
                    SetQueryFunc.setCompanyFilter(newParams);
                    SetQueryFunc.setEstimatedLowerFilter(newParams);
                    SetQueryFunc.setEstimatedGreaterFilter(newParams);
                    SetQueryFunc.setDeliveredLowerFilter(newParams);
                    SetQueryFunc.setDeliveredGreaterFilter(newParams);
                    SetQueryFunc.setWaybillFilter(newParams);

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
