import Image from "next/image";
import PropTypes from "prop-types";
import { Offcanvas, Container, Row, Col, Button, Form } from "react-bootstrap";
import { useState, useEffect } from "react";
import { useFormState } from "react-dom";
import Toastes from "@/components/smaller_components/toast";
import { useRouter } from "next/navigation";
import getOrgsList from "@/utils/documents/get_orgs_list";
import CloseIcon from "../../../../public/icons/close_black.png";
import ErrorMessage from "@/components/smaller_components/error_message";
import addOutsideItems from "@/utils/outside_items/add_outside_items";

/**
 * Create offcanvas that allow to import outside items from file.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showOffcanvas Offcanvas show parameter. If true is visible, if false hidden.
 * @param {Function} props.hideFunction Function that set show parameter to false.
 * @return {JSX.Element} Offcanvas element
 */
function AddOutsideItemsOffcanvas({ showOffcanvas, hideFunction }) {
  const router = useRouter();
  // download data holder
  const [orgs, setOrgs] = useState({
    restOrgs: [],
  });
  // download error
  const [errorDownload, setDownloadError] = useState(false);
  // download data
  useEffect(() => {
    if (showOffcanvas) {
      getOrgsList().then((data) => {
        if (data !== null) {
          setDownloadError(false);
          setOrgs(data);
        } else {
          setDownloadError(true);
        }
      });
    }
  }, [showOffcanvas]);
  // File holder
  const [file, setFile] = useState();
  // Form errors
  const [documentError, setDocumentError] = useState(false);
  // True if create action is running
  const [isLoading, setIsLoading] = useState(false);
  // Form action
  const [state, addItemsAction] = useFormState(
    addOutsideItems.bind(null, file),
    {
      error: false,
      completed: false,
      message: "",
    },
  );
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
        <Offcanvas.Header className="border-bottom-grey px-xl-5">
          <Container className="px-3" fluid>
            <Row>
              <Col xs="9" className="d-flex align-items-center">
                <p className="blue-main-text h4 mb-0">Add outside items</p>
              </Col>
              <Col xs="3" className="text-end pe-0">
                <Button
                  variant="as-link"
                  onClick={() => {
                    hideFunction();
                    if (!state.error && state.completed) {
                      router.refresh();
                    }
                    resetState();
                  }}
                  className="pe-0"
                >
                  <Image src={CloseIcon} alt="Close" />
                </Button>
              </Col>
            </Row>
          </Container>
        </Offcanvas.Header>
        <Offcanvas.Body className="px-4 px-xl-5 pb-0 scrollableHeight" as="div">
          <Container className="p-0" fluid>
            <Form
              className="mx-1 pb-5 mx-xl-3"
              id="outsideItemForm"
              action={addItemsAction}
            >
              <ErrorMessage
                message="Could not download organizations."
                messageStatus={errorDownload}
              />
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">Organization</Form.Label>
                <Form.Select
                  id="orgSelect"
                  className="input-style shadow-sm maxInputWidth"
                  name="org"
                >
                  {Object.values(orgs.restOrgs).map((value) => {
                    return (
                      <option key={value.orgId} value={value.orgId}>
                        {value.orgName}
                      </option>
                    );
                  })}
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-4">
                <Form.Label className="blue-main-text">Currency:</Form.Label>
                <Form.Select
                  id="orgSelect"
                  className="input-style shadow-sm maxInputWidth"
                  name="currency"
                >
                  <option value="PLN">PLN</option>
                  <option value="EUR">EUR</option>
                  <option value="USD">USD</option>
                </Form.Select>
              </Form.Group>
              <Form.Group className="mb-4 maxInputWidth">
                <Form.Label className="blue-main-text">File:</Form.Label>
                <ErrorMessage
                  message="Must be a csv file or not empty."
                  messageStatus={documentError}
                />
                <Form.Control
                  type="file"
                  accept=".csv"
                  isInvalid={documentError}
                  onChange={(e) => {
                    if (e.target.value.endsWith("csv")) {
                      setDocumentError(false);
                      let formData = new FormData();
                      formData.append("file", e.target.files[0]);
                      setFile(formData);
                    } else {
                      setDocumentError(true);
                    }
                  }}
                />
              </Form.Group>
              <Container className="px-0 pb-4 blue-main-text" fluid>
                <p>
                  The file must be a comma separated UTF-8 csv file and must
                  contain columns:
                </p>
                <ul>
                  <li>Partnumber</li>
                  <li>Item_name</li>
                  <li>Item_desc*</li>
                  <li>Ean*</li>
                  <li>Qty</li>
                  <li>Price</li>
                </ul>
                <p>
                  Column with * can be empty. Column Ean can have multiple
                  variables. If column has more then one ean please use the
                  comma character to separate them. For example: 1234,124234.
                </p>
              </Container>
              <Container className="main-grey-bg p-3 fixed-bottom w-100" fluid>
                <Row
                  style={maxStyle}
                  className="mx-auto minScalableWidth offcanvasButtonsStyle"
                >
                  <Col>
                    <Button
                      variant="mainBlue"
                      className="w-100"
                      type="submit"
                      disabled={documentError || errorDownload}
                      onClick={(e) => {
                        e.preventDefault();
                        if (!file) {
                          setDocumentError(true);
                          return;
                        }
                        setIsLoading(true);

                        let form = document.getElementById("outsideItemForm");
                        form.requestSubmit();
                      }}
                    >
                      {isLoading && !state.completed ? (
                        <div className="spinner-border main-text"></div>
                      ) : (
                        "Add"
                      )}
                    </Button>
                  </Col>
                  <Col>
                    <Button
                      variant="red"
                      className="w-100"
                      onClick={() => {
                        hideFunction();
                        if (!state.error && state.completed) {
                          router.refresh();
                        }
                        resetState();
                      }}
                    >
                      Cancel
                    </Button>
                  </Col>
                </Row>
              </Container>
            </Form>
          </Container>
        </Offcanvas.Body>

        <Toastes.ErrorToast
          showToast={state.completed && state.error}
          message={state.message}
          onHideFun={() => {
            resetState();
          }}
        />
        <Toastes.SuccessToast
          showToast={state.completed && !state.error}
          message={state.message}
          onHideFun={() => {
            resetState();
            document.getElementById("outsideItemForm").reset();
            hideFunction();
            router.refresh();
          }}
        />
      </Container>
    </Offcanvas>
  );

  /**
   * Reset form state
   */
  function resetState() {
    state.error = false;
    state.completed = false;
    state.message = "";
    setIsLoading(false);
  }
}

AddOutsideItemsOffcanvas.propTypes = {
  showOffcanvas: PropTypes.bool.isRequired,
  hideFunction: PropTypes.func.isRequired,
};

export default AddOutsideItemsOffcanvas;
