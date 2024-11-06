"use server";

import PropTypes from "prop-types";
import InvoiceMenu from "@/components/menu/wholeMenu/invoice_menu";
import InvoiceList from "@/components/object_list/invoice_list";
import WholeFooter from "@/components/footer/whole_footers/whole_footer";
import getRole from "@/utils/auth/get_role";
import getBasicInfo from "@/utils/menu/get_basic_user_info";
import getNotificationCounter from "@/utils/menu/get_notification_counter";
import getOrgView from "@/utils/auth/get_org_view";
import getYoursInvoices from "@/utils/documents/get_yours_invoices";
import getSearchYoursInvoices from "@/utils/documents/get_yours_invoices_search";
import getSearchSalesInvoices from "@/utils/documents/get_sales_invoices_search";
import getSalesInvoices from "@/utils/documents/get_sales_invoices";
import getSearchCreditNotes from "@/utils/documents/get_credit_note_with_search";
import getCreditNotes from "@/utils/documents/get_credit_note";
import getRequests from "@/utils/documents/get_requests";
import getSearchRequests from "@/utils/documents/get_search_request";

/**
 * Download document data depended on parameters
 * @param {string} docType Which document type (Sales invoices, Yours credit notes, Client credit notes, Requests) should be downloaded. Default is invoices.
 * @param {boolean} org_view True if org view is enabled
 * @param {string} search Value of searched phrase
 * @param {string} currentSort Current sort value
 * @param {Object} params Filter parameters
 * @return {Promise<Array<Object>>} Array containing chosen type of documents
 */
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

/**
 * Document page
 * @param {Object} props
 * @param {Object} props.searchParams Object that gives access to query parameters
 * @param {string} props.searchParams.page Value that determines on which page user is in
 * @param {string} props.searchParams.pagination Value that determines page pagination
 * @param {string} props.searchParams.isOrg Value that determines if org view is enabled
 * @param {string} props.searchParams.searchQuery Filled with value searched in objects
 * @param {string} props.searchParams.orderBy Sort value
 * @param {string} props.searchParams.dateL Date lower then filter
 * @param {string} props.searchParams.dateG Date greater then filter
 * @param {string} props.searchParams.dueL Due date lower then filter
 * @param {string} props.searchParams.dueG Due date greater then filter
 * @param {string} props.searchParams.qtyL Qty lower then filter
 * @param {string} props.searchParams.qtyG Qty greater then filter
 * @param {string} props.searchParams.totalL Total lower then filter
 * @param {string} props.searchParams.totalG Total greater then filter
 * @param {string} props.searchParams.recipient Recipient filter
 * @param {string} props.searchParams.currency Currency filter
 * @param {string} props.searchParams.paymentStatus Payment status filter
 * @param {string} props.searchParams.status Status filter
 * @param {string} props.searchParams.type Type filter
 * @param {string} props.searchParams.requestStatus Request status filter
 * @param {string} props.searchParams.docType Current document type
 */
async function InvoicesPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_notification_qty = await getNotificationCounter();
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
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
    // Check if current role can access the page with org view enabled
  let org_view = getOrgView(current_role, orgActivated === "true");
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let docType = searchParams.docType ? searchParams.docType : "Yours invoices";
  // Download
  let invoices = isSearchTrue
    ? await getDocuments(
        docType,
        org_view,
        searchParams.searchQuery,
        currentSort,
        params,
      )
    : await getDocuments(docType, org_view, null, currentSort, params);
  // Pagination, default 10
  let invoiceLength = invoices ? invoices.length : 0;
  let maxInstanceOnPage = searchParams.pagination ? searchParams.pagination : 10;
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
        current_notification_qty={current_notification_qty}
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

  /**
   * Checks if any filter or sort has been activated
   * @return {boolean}
   */
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
  searchParams: PropTypes.object,
};

export default InvoicesPage;
