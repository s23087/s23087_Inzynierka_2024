"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import { useEffect, useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams, useRouter } from "next/navigation";
import DeleteObjectWindow from "../windows/delete_object";
import InvoiceContainer from "../object_container/documents/yours_invoice_container";
import CreditNoteContainer from "../object_container/documents/credit_note_container";
import RequestContainer from "../object_container/documents/request_container";
import AddInvoiceOffcanvas from "../offcanvas/create/documents/create_invoice";
import deleteInvoice from "@/utils/documents/delete_invoice";
import SelectComponent from "../smaller_components/select_component";
import getPaginationInfo from "@/utils/flexible/get_page_info";
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
import ChangeSelectedStatusWindow from "../windows/change_request_status";

/**
 * Checks if value is selected
 * @param {string} type Name of current chosen type
 * @param {Object} document Document object
 * @param {Array<Object>} selected Array containing selected ids
 * @returns {boolean} True if value exist in given selected array, otherwise false.
 */
function getIfSelected(type, document, selected) {
  if (type.includes("note")) {
    return selected.indexOf(document.creditNoteId) !== -1;
  }
  if (type === "Requests") {
    return selected.indexOf(document.id) !== -1;
  }
  return selected.indexOf(document.invoiceId) !== -1;
}

/**
 * Return component that showcase objects of chosen type, search bar, filter, more action element and selected element.
 * @component
 * @param {object} props Component props
 * @param {Array<Object>} props.invoices Array containing objects of current type.
 * @param {string} props.type Name of chosen type of document.
 * @param {boolean} props.orgView True if org view is enabled.
 * @param {Number} props.invoiceStart Starting index of documents subarray.
 * @param {Number} props.invoiceEnd Ending index of documents subarray.
 * @param {string} props.role Role of current user.
 * @param {boolean} props.filterActive If filter is activated then true, otherwise false.
 * @param {string} props.currentSort Current value of "sort" query parameter
 * @return {JSX.Element} Container element
 */
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
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  // View invoice
  const [showViewInvoice, setShowViewInvoice] = useState(false); // useState for showing view invoice offcanvas
  const [invoiceToView, setInvoiceToView] = useState({
    invoiceDate: "",
    dueDate: "",
    clientName: "",
    inSystem: false,
    paymentStatus: "",
  }); // holder of invoice chosen to view
  // View credit note
  const [showViewCredit, setShowViewCredit] = useState(false); // useState for showing view credit note offcanvas
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
  }); // holder of credit note chosen to view
  // View request
  const [showViewRequest, setShowViewRequest] = useState(false); // useState for showing view request offcanvas
  const [requestToView, setRequestToView] = useState({
    id: 0,
    username: "",
    status: "",
    objectType: "",
    creationDate: "",
  }); // holder of request chosen to view
  // Modify invoice
  const [showModifyInvoice, setShowModifyInvoice] = useState(false); // useState for showing modify invoice offcanvas
  const [invoiceToModify, setInvoiceToModify] = useState({
    invoiceNumber: "",
  }); // holder of invoice chosen to modify
  // Modify credit note
  const [showModifyCreditNote, setShowModifyCreditNote] = useState(false); // useState for showing modify credit note offcanvas
  const [creditNoteToModify, setCreditNoteToModify] = useState({
    invoiceNumber: "",
    date: "",
    clientName: "",
    inSystem: "",
    title: "",
  }); // holder of credit note chosen to modify
  // Modify request
  const [showModifyRequest, setShowModifyRequest] = useState(false); // useState for showing modify request offcanvas
  const [requestToModify, setRequestToModify] = useState({
    id: 0,
    objectType: "",
  }); // holder of request chosen to modify
  // Delete credit note
  const [showDeleteCreditNote, setShowDeleteCreditNote] = useState(false); // useState for showing delete credit note window
  const [creditNoteToDelete, setCreditNoteToDelete] = useState(null); // holder of credit note chosen to delete
  // Delete credit note
  const [showDeleteRequest, setShowDeleteRequest] = useState(false); // useState for showing delete request window
  const [requestToDelete, setRequestToDelete] = useState(null); // holder of request chosen to delete
  // Delete invoice
  const [showDeleteInvoice, setShowDeleteInvoice] = useState(false); // useState for showing delete invoice window
  const [invoiceToDelete, setInvoiceToDelete] = useState(null); // holder of invoice chosen to delete
  // Delete error
  const [isErrorDelete, setIsErrorDelete] = useState(false); // true if delete action failed
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false); // useState for showing more action window
  const [isShowAddInvoice, setShowAddInvoice] = useState(false); // useState for showing create invoice offcanvas
  const [isShowAddCreditNote, setShowAddCreditNote] = useState(false); // useState for showing create credit note offcanvas
  const [isShowAddRequest, setShowAddRequest] = useState(false); // useState for showing create request offcanvas
  // Change request status
  const [showCompleteWindow, setShowCompleteWindow] = useState(false); // useState for showing complete request window
  const [requestToComplete, setRequestToComplete] = useState(); // holds request that user wants to complete
  const [showRejectWindow, setShowRejectWindow] = useState(false); // useState for showing reject request window
  const [requestToReject, setRequestToReject] = useState(); // holds request that user wants to reject
  const [showErrorToast, setShowErrorToast] = useState(false); // useState for showing error toast if change action dropped error
  // Selected
  const [selectedQty, setSelectedQty] = useState(0); // Number of selected objects
  const [selectedDocuments] = useState([]); // Selected documents keys
  // Filter
  const [showFilter, setShowFilter] = useState(false); // useState for showing filter offcanvas
  // mass action
  const [showDeleteSelected, setShowDeleteSelected] = useState(false); // useState for showing mass action delete window
  const [deleteSelectedErrorMess, setDeleteSelectedErrorMess] = useState(""); // error message of mass delete action
  const [showChangeRequestStatus, setShowChangeRequestStatus] = useState(false); // useState for showing mass change request status
  // Clear selected on type change
  useEffect(() => {
    setSelectedQty(0);
    selectedDocuments.shift(0, selectedDocuments.length);
  }, [type]);
  // Styles
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="px-0 middleSectionPlacement position-relative" fluid>
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
        actionOneName={getActionOneName()}
        actionOne={getActionOne()}
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
            return getDocument(value);
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName={type !== "Requests" ? "document" : "request"}
        addAction={() => {
          if (
            (orgView || role === "Admin" || role === "Solo") &&
            type.includes("invoice")
          ) {
            setShowMoreAction(false);
            setShowAddInvoice(true);
          }
          if (
            (orgView || role === "Admin" || role === "Solo") &&
            type.includes("note")
          ) {
            setShowMoreAction(false);
            setShowAddCreditNote(true);
          }
          if ((!orgView || role === "Admin") && type === "Requests") {
            setShowMoreAction(false);
            setShowAddRequest(true);
          }
          if (
            role === "Merchant" &&
            (type.includes("invoice") || type.includes("note"))
          ) {
            setShowMoreAction(false);
            setShowAddRequest(true);
          }
        }}
        selectAllOnPage={() => {
          selectedDocuments.splice(0, selectedDocuments.length);
          setSelectedQty(0);
          let paginationInfo = getPaginationInfo(params);
          Object.values(invoices ?? [])
            .slice(paginationInfo.start, paginationInfo.end)
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
          Object.values(invoices ?? []).forEach((e) => {
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
          setShowDeleteSelected(false);
          setDeleteSelectedErrorMess("");
          setIsErrorDelete(false);
        }}
        instanceName={type.substring(0, type.length - 1)}
        deleteItemFunc={async () => {
          let failures = [];
          for (let index = 0; index < selectedDocuments.length; index++) {
            let result;
            if (type.includes("invoice"))
              result = await deleteInvoice(
                selectedDocuments[index],
                type === "Yours invoices",
              );
            if (type.includes("note"))
              result = await deleteCreditNote(
                selectedDocuments[index],
                type === "Yours credit notes",
              );
            if (type.includes("Requests"))
              result = await deleteRequest(selectedDocuments[index]);
            if (result.error) {
              failures.push(selectedDocuments[index]);
            } else {
              selectedDocuments.splice(index, 1);
              setSelectedQty(selectedDocuments.length);
            }
          }
          if (failures.length === 0) {
            setShowDeleteSelected(false);
            setDeleteSelectedErrorMess("");
            router.refresh();
          } else {
            setDeleteSelectedErrorMess(
              `Error: Could not delete this ${type} (${failures.join(",")}).`,
            );
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
      <ChangeSelectedStatusWindow
        modalShow={showChangeRequestStatus}
        onHideFunction={() => setShowChangeRequestStatus(false)}
        requestsIds={selectedDocuments}
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
        message="Note has exceed the max length."
        onHideFun={() => setShowErrorToast(false)}
      />
    </Container>
  );

  /**
   * Return name of mass action depend on the chosen type
   */
  function getActionOneName() {
    return type === "Requests" ? "Change status" : "Delete selected";
  }

  /**
   * Return function of mass action depend on the chosen type
   */
  function getActionOne() {
    return type === "Requests"
      ? () => setShowChangeRequestStatus(true)
      : () => setShowDeleteSelected(true);
  }

  /**
   * Return element that visually represent value using correct format determent by type
   * @param {Object} value
   * @return {JSX.Element}
   */
  function getDocument(value) {
    if (type.includes("note")) {
      return (
        <CreditNoteContainer
          key={value.creditNoteId}
          credit_note={value}
          is_org={orgView}
          selected={getIfSelected(type, value, selectedDocuments)}
          is_user_type={type.includes("yours")}
          selectAction={() => selectAction(value)}
          unselectAction={() => unselectAction(value)}
          viewAction={() => viewAction(value)}
          deleteAction={() => deleteAction(value)}
          modifyAction={() => modifyAction(value)}
        />
      );
    }
    if (type.includes("Requests")) {
      return (
        <RequestContainer
          key={value.id}
          request={value}
          is_org={orgView}
          selected={getIfSelected(type, value, selectedDocuments)}
          selectAction={() => selectAction(value)}
          unselectAction={() => unselectAction(value)}
          viewAction={() => viewAction(value)}
          deleteAction={() => deleteAction(value)}
          modifyAction={() => modifyAction(value)}
          completeAction={() => completeAction(value)}
          rejectAction={() => rejectAction(value)}
        />
      );
    }

    return (
      <InvoiceContainer
        key={value.invoiceId}
        invoice={value}
        is_org={orgView}
        selected={getIfSelected(type, value, selectedDocuments)}
        is_user_type={type.includes("Yours")}
        selectAction={() => selectAction(value)}
        unselectAction={() => unselectAction(value)}
        deleteAction={() => deleteAction(value)}
        viewAction={() => viewAction(value)}
        modifyAction={() => modifyAction(value)}
      />
    );
  }

  /**
   * Set value of request to reject to chosen value id and then open the reject window
   * @param {Object} value Document value
   */
  function rejectAction(value) {
    if (type === "Requests") {
      setRequestToReject(value.id);
      setShowRejectWindow(true);
    }
  }

  /**
   * Set value of request to complete to chosen value id and then open the complete window
   * @param {Object} value Document value
   */
  function completeAction(value) {
    if (type === "Requests") {
      setRequestToComplete(value.id);
      setShowCompleteWindow(true);
    }
  }

  /**
   * Set modify value to chosen value and then open the modify offcanvas
   * @param {Object} value Document value
   */
  function modifyAction(value) {
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
  }

  /**
   * Set view value to chosen value and then open the view offcanvas
   * @param {Object} value Document value
   */
  function viewAction(value) {
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
  }

  /**
   * Set delete value to id of chosen value and then open the delete window
   * @param {Object} value Document value
   */
  function deleteAction(value) {
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
  }

  /**
   * Delete chosen value from select array using value id.
   * @param {Object} value Document value
   */
  function unselectAction(value) {
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
  }

  /**
   * Add chosen value id to select array
   * @param {Object} value Document value
   */
  function selectAction(value) {
    setSelectedQty(selectedQty + 1);
    if (type.includes("notes")) {
      selectedDocuments.push(value.creditNoteId);
    }
    if (type.includes("invoice")) {
      selectedDocuments.push(value.invoiceId);
    }
    if (type === "Requests") {
      selectedDocuments.push(value.id);
    }
  }
}

InvoiceList.propTypes = {
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
