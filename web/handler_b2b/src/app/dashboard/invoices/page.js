"use server";

import PropTypes from "prop-types";
import InvoiceMenu from "@/components/menu/wholeMenu/invoice_menu";
import InvoiceList from "@/components/object_list/invoice_list";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_nofication_counter";
import getOrgView from "@/utils/auth/get_org_view";
import getYoursInvoices from "@/utils/documents/get_yours_invoices";
import getSearchYoursInvoices from "@/utils/documents/get_yours_invoices_search";
import getSearchSalesInvoices from "@/utils/documents/get_sales_invoices_search";
import getSalesInvoices from "@/utils/documents/get_sales_invoices";
import getSearchCreditNotes from "@/utils/documents/get_credit_note_with_search";
import getCreditNotes from "@/utils/documents/get_credit_note";
import getRequests from "@/utils/documents/get_requests";
import getSearchRequests from "@/utils/documents/get_search_request";

async function getDocuments(docType, org_view, search, currentSort, params) {
  switch (docType) {
    case "Sales invoices":
      if (search) {
        return await getSearchSalesInvoices(
          org_view,
          search,
          currentSort,
          params,
        );
      }
      return await getSalesInvoices(org_view, currentSort, params);
    case "Yours credit notes":
      if (search) {
        return await getSearchCreditNotes(
          org_view,
          true,
          search,
          currentSort,
          params,
        );
      }
      return await getCreditNotes(org_view, true, currentSort, params);
    case "Client credit notes":
      if (search) {
        return await getSearchCreditNotes(
          org_view,
          false,
          search,
          currentSort,
          params,
        );
      }
      return await getCreditNotes(org_view, false, currentSort, params);
    case "Requests":
      if (search) {
        return await getSearchRequests(
          org_view,
          search,
          currentSort,
          params.dateL,
          params.dateG,
          params.type,
          params.requestStatus,
        );
      }
      return await getRequests(
        org_view,
        currentSort,
        params.dateL,
        params.dateG,
        params.type,
        params.requestStatus,
      );
  }
  if (search) {
    return await getSearchYoursInvoices(org_view, search, currentSort, params);
  }

  return await getYoursInvoices(org_view, currentSort, params);
}

async function InvoicesPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  const is_org_switch_needed = current_role == "Admin";
  // Filters
  let currentSort = searchParams.orderBy ?? ".None";
  let params = {
    dateL: searchParams.dateL,
    dateG: searchParams.dateG,
    dueL: searchParams.dueL,
    dueG: searchParams.dueG,
    qtyL: searchParams.qtyL,
    qtyG: searchParams.qtyG,
    totalL: searchParams.totalL,
    totalG: searchParams.totalG,
    recipient: searchParams.recipient,
    currency: searchParams.currency,
    paymentStatus: searchParams.paymentStatus,
    status: searchParams.status,
    type: searchParams.type,
    requestStatus: searchParams.requestStatus,
  };
  let filterActivated = getFilterActivated();
  // Rest
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
  let org_view = getOrgView(current_role, orgActivated === "true");
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let docType = searchParams.docType ? searchParams.docType : "Yours invoices";
  let invoices = isSearchTrue
    ? await getDocuments(
        docType,
        org_view,
        searchParams.searchQuery,
        currentSort,
        params,
      )
    : await getDocuments(docType, org_view, null, currentSort, params);
  let invoiceLength = invoices ? invoices.length : 0;
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let pageQty = Math.ceil(invoiceLength / maxInstanceOnPage);
  pageQty = pageQty === 0 ? 1 : pageQty;
  let currentPage = parseInt(searchParams.page)
    ? parseInt(searchParams.page)
    : 1;
  let invoiceStart = currentPage * maxInstanceOnPage - maxInstanceOnPage;
  invoiceStart = invoiceStart < 0 ? 0 : invoiceStart;
  let invoiceEnd = invoiceStart + maxInstanceOnPage;

  return (
    <main className="d-flex flex-column h-100">
      <InvoiceMenu
        type={docType}
        current_role={current_role}
        current_nofitication_qty={current_nofitication_qty}
        is_org_switch_needed={is_org_switch_needed}
        org_view={org_view}
        user={userInfo}
      />

      <section className="h-100 z-0">
        <InvoiceList
          invoices={invoices}
          type={docType}
          orgView={org_view}
          invoiceStart={invoiceStart}
          invoiceEnd={invoiceEnd}
          role={current_role}
          filterActive={filterActivated}
          currentSort={currentSort}
        />
      </section>

      <WholeFooter
        max_instance_on_page={maxInstanceOnPage}
        page_qty={pageQty}
        current_page={currentPage}
      />
    </main>
  );

  function getFilterActivated() {
    return (
      searchParams.orderBy ||
      params.dateL ||
      params.dateG ||
      params.dueL ||
      params.dueG ||
      params.qtyL ||
      params.qtyG ||
      params.totalL ||
      params.totalG ||
      params.recipient ||
      params.currency ||
      params.paymentStatus ||
      params.status ||
      params.requestStatus ||
      params.type
    );
  }
}

InvoicesPage.propTypes = {
  searchParams: PropTypes.object
}

export default InvoicesPage
