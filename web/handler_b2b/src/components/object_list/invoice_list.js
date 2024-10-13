"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import { useEffect, useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams } from "next/navigation";
import DeleteObjectWindow from "../windows/delete_object";
import { useRouter } from "next/navigation";
import InvoiceContainer from "../object_container/documents/yours_invoice_container";
import CreditNoteContainer from "../object_container/documents/credit_note_contianter";
import RequestContainer from "../object_container/documents/request_container";
import AddInvoiceOffcanvas from "../offcanvas/create/documents/create_invoice";
import deleteInvoice from "@/utils/documents/delete_invoice";
import SelectComponent from "../smaller_components/select_compontent";
import getPagationInfo from "@/utils/flexible/get_page_info";
import ViewInvoiceOffcanvas from "../offcanvas/view/documents/view_invoice";
import ModifyInvoiceOffcanvas from "../offcanvas/modify/documents/modify_invoice";
import AddCreditNoteOffcanvas from "../offcanvas/create/documents/create_credit_note";
import ViewCreditNoteOffcanvas from "../offcanvas/view/documents/view_credit_note";
import deleteCreditNote from "@/utils/documents/delete_credit_note";
import ModifyCreditNoteOffcanvas from "../offcanvas/modify/documents/modify_credit_note";
import AddRequestOffcanvas from "../offcanvas/create/documents/create_request";
import ViewRequestOffcanvas from "../offcanvas/view/documents/view_request";
import deleteRequest from "@/utils/documents/delete_request";
import ModifyRequestOffcanvas from "../offcanvas/modify/documents/modify_request";
import ChangeStatusWindow from "../windows/set_status_Window";
import setRequestStatus from "@/utils/documents/set_request_status";
import Toastes from "../smaller_components/toast";
import InvoiceFilterOffcanvas from "../filter/document_filter";
import DeleteSelectedWindow from "../windows/delete_selected";

function getIfSelected(type, document, selected) {
  if (type.includes("note")) {
    return selected.indexOf(document.creditNoteId) !== -1;
  }
  if (type === "Requests") {
    return selected.indexOf(document.id) !== -1;
  }
  return selected.indexOf(document.invoiceId) !== -1;
}

function getDocument(
  type,
  document,
  is_org,
  selected,
  selectAction,
  unselectAction,
  deleteAction,
  viewAction,
  modifyAction,
  completeAction,
  rejectAction,
) {
  if (type.includes("note")) {
    return (
      <CreditNoteContainer
        key={document.creditNoteId}
        credit_note={document}
        is_org={is_org}
        selected={selected}
        is_user_type={type.includes("yours")}
        selectAction={selectAction}
        unselectAction={unselectAction}
        viewAction={viewAction}
        deleteAction={deleteAction}
        modifyAction={modifyAction}
      />
    );
  }
  if (type.includes("Requests")) {
    return (
      <RequestContainer
        key={document.id}
        request={document}
        is_org={is_org}
        selected={selected}
        selectAction={selectAction}
        unselectAction={unselectAction}
        viewAction={viewAction}
        deleteAction={deleteAction}
        modifyAction={modifyAction}
        completeAction={completeAction}
        rejectAction={rejectAction}
      />
    );
  }

  return (
    <InvoiceContainer
      key={document.invoiceId}
      invoice={document}
      is_org={is_org}
      selected={selected}
      is_user_type={type.includes("Yours")}
      selectAction={selectAction}
      unselectAction={unselectAction}
      deleteAction={deleteAction}
      viewAction={viewAction}
      modifyAction={modifyAction}
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
  filterActive,
  currentSort,
}) {
  // View invoice
  const [showViewInvoice, setShowViewInvoice] = useState(false);
  const [invoiceToView, setInvoiceToView] = useState({
    invoiceDate: "",
    dueDate: "",
    clientName: "",
    inSystem: false,
    paymentStatus: "",
  });
  // View credit note
  const [showViewCredit, setShowViewCredit] = useState(false);
  const [creditToView, setCreditToView] = useState({
    user: "",
    creditNoteId: 0,
    invoiceNumber: "",
    date: "",
    qty: 0,
    total: 0,
    clientName: 0,
    inSystem: false,
    isPaid: false,
  });
  // View request
  const [showViewRequest, setShowViewRequest] = useState(false);
  const [requestToView, setRequestToView] = useState({
    id: 0,
    username: "",
    status: "",
    objectType: "",
    creationDate: "",
  });
  // Modify invoice
  const [showModifyInvoice, setShowModifyInvoice] = useState(false);
  const [invoiceToModify, setInvoiceToModify] = useState({
    invoiceNumber: "",
  });
  // Modify credit note
  const [showModifyCreditNote, setShowModifyCreditNote] = useState(false);
  const [creditNoteToModify, setCreditNoteToModify] = useState({
    invoiceNumber: "",
    date: "",
    clientName: "",
    inSystem: "",
    title: "",
  });
  // Modify request
  const [showModifyRequest, setShowModifyRequest] = useState(false);
  const [requestToModify, setRequestToModify] = useState({
    id: 0,
    objectType: "",
  });
  // Delete credit note
  const [showDeleteCreditNote, setShowDeleteCreditNote] = useState(false);
  const [creditNoteToDelete, setCreditNoteToDelete] = useState(null);
  // Delete credit note
  const [showDeleteRequest, setShowDeleteRequest] = useState(false);
  const [requestToDelete, setRequestToDelete] = useState(null);
  // Delete invoice
  const [showDeleteInvoice, setShowDeleteInvoice] = useState(false);
  const [invoiceToDelete, setInvoiceToDelete] = useState(null);
  // Delete error
  const [isErrorDelete, setIsErrorDelete] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddInvoice, setShowAddInvoice] = useState(false);
  const [isShowAddCreditNote, setShowAddCreditNote] = useState(false);
  const [isShowAddRequest, setShowAddRequest] = useState(false);
  // Change request status
  const [showCompleteWindow, setShowCompleteWindow] = useState(false);
  const [requestToComplete, setRequestToComplete] = useState();
  const [showRejectWindow, setShowRejectWindow] = useState(false);
  const [requestToReject, setRequestToReject] = useState();
  const [showErrorToast, setShowErrorToast] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedDocuments] = useState([]);
  // Filter
  const [showFilter, setShowFilter] = useState(false);
  // mass action
  const [showDeleteSelected, setShowDeleteSelected] = useState(false);
  const [deleteSelectedErrorMess, setDeleteSelectedErrorMess] = useState("");
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  // Type change
  useEffect(() => {
    setSelectedQty(0);
    selectedDocuments.shift(0, selectedDocuments.length);
  }, [type]);
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="p-0 middleSectionPlacement position-relative" fluid>
      <InvoiceFilterOffcanvas
        showOffcanvas={showFilter}
        hideFunction={() => setShowFilter(false)}
        currentSort={currentSort}
        currentDirection={
          currentSort.startsWith("A") || currentSort.startsWith(".")
        }
        type={type}
      />
      <Container
        className="fixed-top middleSectionPlacement-no-footer p-0"
        fluid
      >
        <SearchFilterBar
          filter_icon_bool={filterActive}
          moreButtonAction={() => setShowMoreAction(true)}
          filterAction={() => setShowFilter(true)}
        />
      </Container>
      <SelectComponent 
        selectedQty={selectedQty} 
        actionOneName="Delete selected"
        actionOne={() => setShowDeleteSelected(true)}
      />
      <Container style={selectedQty > 0 ? containerMargin : null}></Container>
      {Object.keys(invoices ?? []).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">
            {invoices ? `${type} not found :/` : "Could not connect to server."}
          </p>
        </Container>
      ) : (
        Object.values(invoices ?? [])
          .slice(invoiceStart, invoiceEnd)
          .map((value) => {
            return getDocument(
              type,
              value,
              orgView,
              getIfSelected(type, value, selectedDocuments),
              () => {
                // Select
                setSelectedQty(selectedQty + 1);
                if (type.includes("notes")) {
                  selectedDocuments.push(value.creditNoteId);
                  return;
                }
                if (type.includes("invoice")) {
                  selectedDocuments.push(value.invoiceId);
                  return;
                }
                if (type === "Requests") {
                  selectedDocuments.push(value.id);
                  return;
                }
              },
              () => {
                // Unselect
                let index;
                if (type.includes("notes")) {
                  index = selectedDocuments.indexOf(value.creditNoteId);
                }
                if (type.includes("invoice")) {
                  index = selectedDocuments.indexOf(value.invoiceId);
                }
                if (type === "Requests") {
                  index = selectedDocuments.indexOf(value.id);
                }
                selectedDocuments.splice(index, 1);
                setSelectedQty(selectedQty - 1);
              },
              () => {
                // Delete
                if (type.includes("notes")) {
                  setCreditNoteToDelete(value.creditNoteId);
                  setShowDeleteCreditNote(true);
                }
                if (type.includes("invoice")) {
                  setInvoiceToDelete(value.invoiceId);
                  setShowDeleteInvoice(true);
                }
                if (type === "Requests") {
                  setRequestToDelete(value.id);
                  setShowDeleteRequest(true);
                }
              },
              () => {
                // View
                if (type.includes("notes")) {
                  setCreditToView(value);
                  setShowViewCredit(true);
                }
                if (type.includes("invoice")) {
                  setInvoiceToView(value);
                  setShowViewInvoice(true);
                }
                if (type === "Requests") {
                  setRequestToView(value);
                  setShowViewRequest(true);
                }
              },
              () => {
                // Modify
                if (type.includes("notes")) {
                  setCreditNoteToModify(value);
                  setShowModifyCreditNote(true);
                }
                if (type.includes("invoice")) {
                  setInvoiceToModify(value);
                  setShowModifyInvoice(true);
                }
                if (type === "Requests") {
                  setRequestToModify(value);
                  setShowModifyRequest(true);
                }
              },
              () => {
                // Complete
                if (type === "Requests") {
                  setRequestToComplete(value.id);
                  setShowCompleteWindow(true);
                }
              },
              () => {
                // Reject
                if (type === "Requests") {
                  setRequestToReject(value.id);
                  setShowRejectWindow(true);
                }
              },
            );
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName={type !== "Requests" ? "document" : "request"}
        addAction={() => {
          if ((orgView || role === "Admin") && type.includes("invoice")) {
            setShowMoreAction(false);
            setShowAddInvoice(true);
          }
          if ((orgView || role === "Admin") && type.includes("note")) {
            setShowMoreAction(false);
            setShowAddCreditNote(true);
          }
          if ((!orgView || role === "Admin") && type === "Requests") {
            setShowMoreAction(false);
            setShowAddRequest(true);
          }
        }}
        selectAllOnPage={() => {
          selectedDocuments.splice(0, selectedDocuments.length);
          setSelectedQty(0);
          let pagationInfo = getPagationInfo(params);
          Object.values(invoices)
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) => {
              if (type.includes("invoice")) selectedDocuments.push(e.invoiceId);
              if (type.includes("note")) selectedDocuments.push(e.creditNoteId);
              if (type === "Requests") selectedDocuments.push(e.id);
            });
          setSelectedQty(selectedDocuments.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedDocuments.splice(0, selectedDocuments.length);
          setSelectedQty(0);
          Object.values(invoices).forEach((e) => {
            if (type.includes("invoice")) selectedDocuments.push(e.invoiceId);
            if (type.includes("note")) selectedDocuments.push(e.creditNoteId);
            if (type === "Requests") selectedDocuments.push(e.id);
          });
          setSelectedQty(selectedDocuments.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedDocuments.splice(0, selectedDocuments.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
      />
      <AddCreditNoteOffcanvas
        showOffcanvas={isShowAddCreditNote}
        hideFunction={() => setShowAddCreditNote(false)}
        isYourCreditNote={type === "Yours credit notes"}
      />
      <AddInvoiceOffcanvas
        showOffcanvas={isShowAddInvoice}
        hideFunction={() => setShowAddInvoice(false)}
        isYourInvoice={type === "Yours invoices"}
      />
      <AddRequestOffcanvas
        showOffcanvas={isShowAddRequest}
        hideFunction={() => setShowAddRequest(false)}
      />
      <DeleteObjectWindow
        modalShow={showDeleteInvoice}
        onHideFunction={() => {
          setShowDeleteInvoice(false);
          setIsErrorDelete(false);
        }}
        instanceName="invoice"
        instanceId={invoiceToDelete}
        deleteItemFunc={async () => {
          let result = await deleteInvoice(
            invoiceToDelete,
            type === "Yours invoices",
          );
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
      <DeleteObjectWindow
        modalShow={showDeleteCreditNote}
        onHideFunction={() => {
          setShowDeleteCreditNote(false);
          setIsErrorDelete(false);
        }}
        instanceName="document"
        instanceId={creditNoteToDelete}
        deleteItemFunc={async () => {
          let result = await deleteCreditNote(
            creditNoteToDelete,
            type === "Yours credit notes",
          );
          if (!result.error) {
            setShowDeleteCreditNote(false);
            router.refresh();
          } else {
            setErrorMessage(result.message);
            setIsErrorDelete(true);
          }
        }}
        isError={isErrorDelete}
        errorMessage={errorMessage}
      />
      <DeleteObjectWindow
        modalShow={showDeleteRequest}
        onHideFunction={() => {
          setShowDeleteRequest(false);
          setIsErrorDelete(false);
        }}
        instanceName="request"
        instanceId={requestToDelete}
        deleteItemFunc={async () => {
          let result = await deleteRequest(requestToDelete);
          if (!result.error) {
            setShowDeleteRequest(false);
            router.refresh();
          } else {
            setErrorMessage(result.message);
            setIsErrorDelete(true);
          }
        }}
        isError={isErrorDelete}
        errorMessage={errorMessage}
      />
      <DeleteSelectedWindow
        modalShow={showDeleteSelected}
        onHideFunction={() => {
          setShowDeleteSelected(false)
          setDeleteSelectedErrorMess("")
          setIsErrorDelete(false)
        }}
        instanceName={type.substring(0, type.length - 1)}
        deleteItemFunc={async () => {
          let failures = [];
          for (let index = 0; index < selectedDocuments.length; index++) {
            let result;
            if (type.includes("invoice")) result = await deleteInvoice(selectedDocuments[index], type === "Yours invoices");
            if (type.includes("note")) result = await deleteCreditNote(selectedDocuments[index], type === "Yours credit notes");
            if (type.includes("Requests")) result = await deleteRequest(selectedDocuments[index]);
            if (result.error) {
              failures.push(selectedDocuments[index])
            } else {
              selectedDocuments.splice(index, 1)
              setSelectedQty(selectedDocuments.length)
            }
          }
          if (failures.length === 0) {
            setShowDeleteSelected(false);
            setDeleteSelectedErrorMess("")
            router.refresh();
          } else {
            setDeleteSelectedErrorMess(`Error: Could not delete this ${type} (${failures.join(",")}).`)
            setIsErrorDelete(true);
            router.refresh();
          }
        }}
        isError={isErrorDelete}
        errorMessage={deleteSelectedErrorMess}
      />
      <ModifyInvoiceOffcanvas
        showOffcanvas={showModifyInvoice}
        hideFunction={() => setShowModifyInvoice(false)}
        isYourInvoice={type === "Yours invoices"}
        invoice={invoiceToModify}
      />
      <ModifyCreditNoteOffcanvas
        showOffcanvas={showModifyCreditNote}
        hideFunction={() => setShowModifyCreditNote(false)}
        isYourCredit={type === "Yours credit notes"}
        creditNote={creditNoteToModify}
      />
      <ModifyRequestOffcanvas
        showOffcanvas={showModifyRequest}
        hideFunction={() => setShowModifyRequest(false)}
        request={requestToModify}
      />
      <ViewInvoiceOffcanvas
        showOffcanvas={showViewInvoice}
        hideFunction={() => setShowViewInvoice(false)}
        invoice={invoiceToView}
        isYourInvoice={type === "Yours invoices"}
      />
      <ViewCreditNoteOffcanvas
        showOffcanvas={showViewCredit}
        hideFunction={() => setShowViewCredit(false)}
        creditNote={creditToView}
        isYourCredit={type === "Yours credit notes"}
      />
      <ViewRequestOffcanvas
        showOffcanvas={showViewRequest}
        hideFunction={() => setShowViewRequest(false)}
        request={requestToView}
        isOrg={orgView}
      />
      <ChangeStatusWindow
        modalShow={showCompleteWindow}
        onHideFunction={() => setShowCompleteWindow(false)}
        actionName="complete"
        actionFunc={async (val) => {
          let result = await setRequestStatus(
            requestToComplete,
            "Fulfilled",
            val,
          );
          if (result) {
            router.refresh();
          } else {
            setShowErrorToast(true);
          }
        }}
      />
      <ChangeStatusWindow
        modalShow={showRejectWindow}
        onHideFunction={() => setShowRejectWindow(false)}
        actionName="reject"
        actionFunc={async (val) => {
          let result = await setRequestStatus(
            requestToReject,
            "Request cancelled",
            val,
          );
          if (result) {
            router.refresh();
          } else {
            setShowErrorToast(true);
          }
        }}
      />
      <Toastes.ErrorToast
        showToast={showErrorToast}
        message="Note has excceed the max length."
        onHideFun={() => setShowErrorToast(false)}
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
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default InvoiceList;
