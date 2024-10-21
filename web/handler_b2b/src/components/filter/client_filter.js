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
import { useEffect, useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import getCountries from "@/utils/flexible/get_countries";
import ErrorMessage from "../smaller_components/error_message";
import FilterHeader from "./filter_header";

function ClientFilterOffcanvas({
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
  const orderBy = ["Name", "Country"];
  const [countries, setCountries] = useState([]);
  const [errorDownload, setDownloadError] = useState(false);
  useEffect(() => {
    getCountries().then((data) => {
      if (data !== null) {
        setDownloadError(false);
        setCountries(data);
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
              message="Could not download countries."
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
              <Stack className="mt-2">
                <p className="mb-1 blue-sec-text">Country:</p>
                <Container className="px-0">
                  <Form.Select
                    className="input-style"
                    style={maxStyle}
                    id="country"
                    defaultValue={newParams.get("country") ?? "none"}
                  >
                    <option value="none">None</option>
                    {countries.map((value) => {
                      return (
                        <option key={value.id} value={value.id}>
                          {value.countryName}
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
                    let country = document.getElementById("country").value;
                    if (country !== "none") newParams.set("country", country);
                    if (country === "none") newParams.delete("country");

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
                    newParams.delete("country");
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

ClientFilterOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
  currentSort: PropTypes.string.isRequired,
  currentDirection: PropTypes.bool.isRequired,
};

export default ClientFilterOffcanvas;
