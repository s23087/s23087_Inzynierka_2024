"use client";

import PropTypes from "prop-types";
import {
  Container,
  Row,
  Col,
  Dropdown,
  DropdownButton,
  Button,
} from "react-bootstrap";
import { useEffect, useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams } from "next/navigation";
import DeleteObjectWindow from "../windows/delete_object";
import ViewClientOffcanvas from "../offcanvas/view/view_client";
import deleteClient from "@/utils/clients/delete_client";
import { useRouter } from "next/navigation";
import ModifyClientOffcanvas from "../offcanvas/modify/modify_client";
import InvoiceContainer from "../object_container/documents/yours_invoice_container";
import CreditNoteContainer from "../object_container/documents/credit_note_contianter";
import RequestContainer from "../object_container/documents/request_container";
import AddInvoiceOffcanvas from "../offcanvas/create/documents/create_invoice";
import deleteInvoice from "@/utils/documents/delete_invoice";

function getDocument(
  type,
  document,
  is_org,
  selected,
  selectAction,
  unselectAction,
  deleteAction,
) {
  switch (type) {
    case "Sales invoices":
      return (
        <InvoiceContainer
          key={document.invoiceId}
          invoice={document}
          is_org={is_org}
          selected={selected}
          is_user_type={false}
          selectAction={selectAction}
          unselectAction={unselectAction}
          deleteAction={deleteAction}
        />
      );
    case "Yours credit notes":
      return (
        <CreditNoteContainer
          credit_note={document}
          is_org={is_org}
          selected={selected}
          is_user_type={true}
        />
      );
    case "Client credit notes":
      return (
        <CreditNoteContainer
          credit_note={document}
          is_org={is_org}
          selected={selected}
          is_user_type={false}
        />
      );
    case "Requests":
      return (
        <RequestContainer
          request={document}
          is_org={is_org}
          selected={selected}
        />
      );
  }

  return (
    <InvoiceContainer
      key={document.invoiceId}
      invoice={document}
      is_org={is_org}
      selected={selected}
      is_user_type={true}
      selectAction={selectAction}
      unselectAction={unselectAction}
      deleteAction={deleteAction}
    />
  );
}

function InvoiceList({
  invoices,
  type,
  orgView,
  invoiceStart,
  invoiceEnd,
  role,
}) {
  // View client
  const [showViewClient, setShowViewClient] = useState(false);
  const [clientToView, setClientToView] = useState({
    users: [],
  });
  // Modify client
  const [showModifyClient, setShowModifyClient] = useState(false);
  const [clientToModify, setClientToModify] = useState({
    users: [],
  });
  // Delete client
  const [showDeleteInvoice, setShowDeleteInvoice] = useState(false);
  const [invoiceToDelete, setInvoiceToDelete] = useState(null);
  const [isErrorDelete, setIsErrorDelete] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddInvoice, setShowAddInvoice] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedKeys] = useState([]);
  // Selected bar button
  const [isClicked, setIsClicked] = useState(false);
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  const accessibleParams = new URLSearchParams(params);
  // Type change
  useEffect(() => {
    setSelectedQty(0);
    selectedKeys.shift(0, selectedKeys.length);
  }, [type]);
  // Vars and styles
  let pagation = accessibleParams.get("pagation")
    ? accessibleParams.get("pagation")
    : 10;
  let page = accessibleParams.get("page") ? accessibleParams.get("page") : 1;
  const containerStyle = {
    height: "67px",
    display: selectedQty > 0 ? "block" : "none",
  };
  const buttonStyle = {
    width: "154px",
    height: "48px",
  };
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="p-0 middleSectionPlacement position-relative" fluid>
      <Container
        className="fixed-top middleSectionPlacement-no-footer p-0"
        fluid
      >
        <SearchFilterBar
          filter_icon_bool="false"
          moreButtonAction={() => setShowMoreAction(true)}
        />
      </Container>
      <Container
        className="border-bottom-grey fixed-top middleSectionPlacement main-bg minScalableWidth"
        style={containerStyle}
        fluid
      >
        <Row className="h-100 px-xl-3 mx-1 align-items-center">
          <Col>
            <span className="blue-main-text">Selected: {selectedQty}</span>
          </Col>
          <Col>
            <DropdownButton
              className="ms-auto text-end"
              title="Mass action"
              variant={isClicked ? "secBlue" : "mainBlue"}
              style={buttonStyle}
              drop="start"
              onClick={() => {
                setIsClicked(isClicked ? false : true);
              }}
              bsPrefix="w-100 btn"
            >
              <Dropdown.Item>
                <Button variant="as-link">Change attribute</Button>
              </Dropdown.Item>
            </DropdownButton>
          </Col>
        </Row>
      </Container>
      <Container style={selectedQty > 0 ? containerMargin : null}></Container>
      {Object.keys(invoices).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">{type} not found :/</p>
        </Container>
      ) : (
        Object.values(invoices)
          .slice(invoiceStart, invoiceEnd)
          .map((value) => {
            return getDocument(
              type,
              value,
              orgView,
              selectedKeys.indexOf(value.invoiceId) !== -1,
              () => {
                // Select
                selectedKeys.push(value.invoiceId);
                setSelectedQty(selectedQty + 1);
              },
              () => {
                // Unselect
                let index = selectedKeys.indexOf(value.invoiceId);
                selectedKeys.splice(index, 1);
                setSelectedQty(selectedQty - 1);
              },
              () => {
                setInvoiceToDelete(value.invoiceId);
                setShowDeleteInvoice(true);
              },
            );
            //   <ClientContainer
            //     key={value.clientId}
            //     client={value}
            //     is_org={orgView}
            //     selected={selectedKeys.indexOf(value.clientId) !== -1}
            //     viewAction={() => {
            //       setClientToView(value);
            //       setShowViewClient(true);
            //     }}
            //     modifyAction={() => {
            //       setClientToModify(value);
            //       setShowModifyClient(true);
            //     }}
            //   />
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName="document"
        addAction={() => {
          if (orgView || role === "Admin") {
            setShowMoreAction(false);
            setShowAddInvoice(true);
          }
        }}
        selectAllOnPage={() => {
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          let start = page * pagation - pagation;
          let end = page * pagation;
          Object.values(invoices)
            .slice(start, end)
            .forEach((e) => selectedKeys.push(e.invoiceId));
          setSelectedQty(selectedKeys.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          Object.values(invoices).forEach((e) =>
            selectedKeys.push(e.invoiceId),
          );
          setSelectedQty(selectedKeys.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
      />
      <AddInvoiceOffcanvas
        showOffcanvas={isShowAddInvoice}
        hideFunction={() => setShowAddInvoice(false)}
        isYourInvoice={type === "Yours invoices"}
      />
      <DeleteObjectWindow
        modalShow={showDeleteInvoice}
        onHideFunction={() => {
          setShowDeleteInvoice(false);
          setIsErrorDelete(false);
        }}
        instanceName="document"
        instanceId={invoiceToDelete}
        deleteItemFunc={async () => {
          let result = await deleteInvoice(invoiceToDelete);
          if (!result.error) {
            setShowDeleteInvoice(false);
            router.refresh();
          } else {
            setErrorMessage(result.message);
            setIsErrorDelete(true);
          }
        }}
        isError={isErrorDelete}
        errorMessage={errorMessage}
      />
      <ModifyClientOffcanvas
        showOffcanvas={showModifyClient}
        hideFunction={() => setShowModifyClient(false)}
        client={clientToModify}
        isOrg={orgView}
      />
      <ViewClientOffcanvas
        showOffcanvas={showViewClient}
        hideFunction={() => setShowViewClient(false)}
        client={clientToView}
        isOrg={orgView}
      />
    </Container>
  );
}

InvoiceList.PropTypes = {
  invoices: PropTypes.object.isRequired,
  type: PropTypes.string.isRequired,
  orgView: PropTypes.bool.isRequired,
  invoiceStart: PropTypes.number.isRequired,
  invoiceEnd: PropTypes.number.isRequired,
  role: PropTypes.string.isRequired,
};

export default InvoiceList;
