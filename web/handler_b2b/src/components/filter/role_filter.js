import PropTypes from "prop-types";
import {
  Offcanvas,
  Container,
  Row,
  Col,
  Button,
  Stack,
  Form,
} from "react-bootstrap";
import { useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import FilterHeader from "./filter_header";
import SetQueryFunc from "./filters_query_functions";
import SortOrderComponent from "./sort_component";

/**
 * Create offcanvas that allow to filter and sort role objects.
 * @component
 * @param {object} props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @param {string} props.currentSort Current sort value
 * @param {boolean} props.currentDirection True if ascending, false if descending.
 * @param {Array<string>} props.rolesToChoose List of roles.
 * @return {JSX.Element} Offcanvas element
 */
function RoleFilterOffcanvas({
  showOffcanvas,
  hideFunction,
  currentSort,
  currentDirection,
  rolesToChoose,
}) {
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  const newParams = new URLSearchParams(params);
  // True is ascending order is enabled
  const [isAsc, setIsAsc] = useState(currentDirection);
  // Order options
  const orderBy = ["User"];
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
        <FilterHeader hideFunction={hideFunction} />
        <Offcanvas.Body className="px-4 px-xl-5 pb-0" as="div">
          <Container className="p-0 mx-1 mx-xl-3" style={vhStyle} fluid>
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
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Role:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="role"
                    defaultValue={newParams.get("role") ?? "none"}
                  >
                    <option value="none">None</option>
                    {Object.values(rolesToChoose).map((value) => {
                      return (
                        <option key={value} value={value}>
                          {value}
                        </option>
                      );
                    })}
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
                    let role = document.getElementById("role").value;
                    if (role !== "none") newParams.set("role", role);
                    if (role === "none") newParams.delete("role");

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
                    newParams.delete("role");
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

RoleFilterOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
  rolesToChoose: PropTypes.array.isRequired,
};

export default RoleFilterOffcanvas;
