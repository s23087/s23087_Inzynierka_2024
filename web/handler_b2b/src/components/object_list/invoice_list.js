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

function getIfSelected(type, document, selected) {
  if (type.includes("note")) {
    return selected.indexOf(document.creditNoteId) !== -1;
  }
  if (type.includes("Request")) {
    return selected.indexOf(document.requestId) !== -1;
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
      />
    );
  }
  if (type.includes("Requests")) {
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
  // Modify invoice
  const [showModifyInvoice, setShowModifyInvoice] = useState(false);
  const [invoiceToModify, setInvoiceToModify] = useState({
    invoiceNumber: "",
  });
  // Delete credit note
  const [showDeleteCreditNote, setShowDeleteCreditNote] = useState(false);
  const [creditNoteToDelete, setCreditNoteToDelete] = useState(null);
  // Delete invoice
  const [showDeleteInvoice, setShowDeleteInvoice] = useState(false);
  const [invoiceToDelete, setInvoiceToDelete] = useState(null);
  const [isErrorDelete, setIsErrorDelete] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddInvoice, setShowAddInvoice] = useState(false);
  const [isShowAddCreditNote, setShowAddCreditNote] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedDocuments] = useState([]);
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
      <Container
        className="fixed-top middleSectionPlacement-no-footer p-0"
        fluid
      >
        <SearchFilterBar
          filter_icon_bool="false"
          moreButtonAction={() => setShowMoreAction(true)}
        />
      </Container>
      <SelectComponent selectedQty={selectedQty} />
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
              getIfSelected(type, value, selectedDocuments),
              () => {
                // Select
                setSelectedQty(selectedQty + 1);
                if (type.includes("notes")) {
                  selectedDocuments.push(value.creditNoteId);
                  return;
                }
                selectedDocuments.push(value.invoiceId);
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
              },
              () => {
                // Modify
                setInvoiceToModify(value);
                setShowModifyInvoice(true);
              },
            );
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName="document"
        addAction={() => {
          if ((orgView || role === "Admin") && type.includes("invoice")) {
            setShowMoreAction(false);
            setShowAddInvoice(true);
          }
          if ((orgView || role === "Admin") && type.includes("note")) {
            setShowMoreAction(false);
            setShowAddCreditNote(true);
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
      <DeleteObjectWindow
        modalShow={showDeleteInvoice}
        onHideFunction={() => {
          setShowDeleteInvoice(false);
          setIsErrorDelete(false);
        }}
        instanceName="invoice"
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
      <DeleteObjectWindow
        modalShow={showDeleteCreditNote}
        onHideFunction={() => {
          setShowDeleteCreditNote(false);
          setIsErrorDelete(false);
        }}
        instanceName="document"
        instanceId={creditNoteToDelete}
        deleteItemFunc={async () => {
          let result = await deleteCreditNote(creditNoteToDelete);
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
      <ModifyInvoiceOffcanvas
        showOffcanvas={showModifyInvoice}
        hideFunction={() => setShowModifyInvoice(false)}
        isYourInvoice={type === "Yours invoices"}
        invoice={invoiceToModify}
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
