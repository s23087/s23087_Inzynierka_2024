import PropTypes from "prop-types";
import { Container, Row, Col, Button } from "react-bootstrap";
import ContainerButtons from "../smaller_components/container_buttons";
import getFileFormServer from "@/utils/documents/download_file";
import { useState } from "react";

/**
 * Create element that represent proforma object
 * @component
 * @param {Object} props
 * @param {{pricelistId: Number, created: string, status: string, name: string, totalItems: Number, path: string, currency: string, modified: string}} props.pricelist Object value
 * @param {boolean} props.selected True if container should show as selected
 * @param {Function} props.selectAction Action that will activated after clicking select button
 * @param {Function} props.unselectAction Action that will activated after clicking unselect button
 * @param {Function} props.deleteAction Action that will activated after clicking delete button
 * @param {Function} props.viewAction Action that will activated after clicking view button
 * @param {Function} props.modifyAction Action that will activated after clicking modify button
 * @return {JSX.Element} Container element
 */
function PricelistContainer({
  pricelist,
  selected,
  selectAction,
  unselectAction,
  deleteAction,
  viewAction,
  modifyAction,
}) {
  let type = pricelist.path.substring(
    pricelist.path.lastIndexOf(".") + 1,
    pricelist.path.lenght,
  );
  // Download bool
  const [isDownloading, setIsDownloading] = useState(false);
  // styles
  const containerBg = {
    backgroundColor: "var(--sec-blue)",
  };
  const statusBgStyle = {
    backgroundColor:
      pricelist.status === "Active" ? "var(--main-green)" : "var(--sec-red)",
    color: pricelist.status === "Active" ? null : "var(--text-main-color)",
    minWidth: "159px",
    minHeight: "25px",
    alignItems: "center",
  };
  const actionButtonStyle = {
    height: "48px",
    maxWidth: "369px",
  };
  return (
    <Container
      className="py-3 px-4 px-xl-5 black-text medium-text border-bottom-grey"
      style={selected ? containerBg : null}
      fluid
    >
      <Row>
        <Col xs="12" md="7" lg="5" xxl="4">
          <Row className="gy-2">
            <Col className="pe-1" xs="auto">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">
                  Created: {pricelist.created.substring(0, 10)}
                </p>
              </span>
            </Col>
            <Col className="ps-1">
              <span className=" d-flex rounded-span px-2" style={statusBgStyle}>
                <p className="mb-0">Status: {pricelist.status}</p>
              </span>
            </Col>
            <Col xs="12">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 d-block text-truncate">
                <p className="mb-0">Name: {pricelist.name}</p>
              </span>
            </Col>
            <Col className="pe-1" xs="auto">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">Type: {type}</p>
              </span>
            </Col>
            <Col className="ps-1">
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2">
                <p className="mb-0">
                  Modified: {pricelist.modified.substring(0, 10)}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col xs="12" md="5" lg="4" xxl="2" className="mt-2 mt-xl-0">
          <Row className="gy-2 h-100 align-items-center">
            <Col xs="12">
              <span className="spanStyle main-blue-bg main-text d-flex rounded-span px-2">
                <p className="mb-0">Products offered: {pricelist.totalItems}</p>
              </span>
              <span className="spanStyle main-grey-bg d-flex rounded-span px-2 mt-2">
                <p className="mb-0 text-nowrap overflow-x-hidden">
                  Currency: {pricelist.currency}
                </p>
              </span>
            </Col>
          </Row>
        </Col>
        <Col xs="12" lg="3" xxl="2" className="mt-2 mt-md-3 mt-lg-0">
          <Row className="gy-2 h-100 align-items-center text-center">
            <Col xs="12">
              <Button
                variant="mainBlue"
                className="w-100"
                style={actionButtonStyle}
                disabled={isDownloading || pricelist.status === "Deactivated"}
                onClick={async () => {
                  if (!pricelist.path.endsWith("csv")) {
                    window.open(
                      pricelist.path.substring(7, pricelist.path.length),
                    );
                    return;
                  }
                  setIsDownloading(true);
                  let file = await getFileFormServer(pricelist.path);
                  if (file) {
                    let parsed = JSON.parse(file);
                    let buffer = Buffer.from(parsed.data);
                    let blob = new Blob([buffer], { type: "application/csv" });
                    let url = URL.createObjectURL(blob);
                    let downloadObject = document.createElement("a");
                    downloadObject.href = url;
                    downloadObject.download = pricelist.path.substring(
                      pricelist.path.lastIndexOf("/"),
                      pricelist.path.length,
                    );
                    downloadObject.click();
                  }
                  setIsDownloading(false);
                }}
              >
                {type === "csv" ? "Download file" : "Open link to xml"}
              </Button>
            </Col>
          </Row>
        </Col>
        <Col xs="12" xxl="4" className="px-0 pt-3 pt-xl-3 pt-xxl-0 pb-2">
          <ContainerButtons
            selected={selected}
            deleteAction={deleteAction}
            selectAction={selectAction}
            unselectAction={unselectAction}
            viewAction={viewAction}
            modifyAction={modifyAction}
          />
        </Col>
      </Row>
    </Container>
  );
}

PricelistContainer.propTypes = {
  pricelist: PropTypes.object.isRequired,
  selected: PropTypes.bool.isRequired,
  selectAction: PropTypes.func,
  unselectAction: PropTypes.func,
  deleteAction: PropTypes.func,
  viewAction: PropTypes.func,
  modifyAction: PropTypes.func,
};

export default PricelistContainer;
