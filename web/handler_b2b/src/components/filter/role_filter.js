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
} from "react-bootstrap";
import CloseIcon from "../../../public/icons/close_black.png";
import { useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

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
  const [isAsc, setIsAsc] = useState(currentDirection);
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

RoleFilterOffcanvas.PropTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
};

export default RoleFilterOffcanvas;
