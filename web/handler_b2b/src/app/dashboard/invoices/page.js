"use server";

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

async function getDocuments(type, org_view, search) {
  switch (type) {
    case "Sales invoices":
      if (search) {
        return await getSearchSalesInvoices(org_view, search);
      }
      return await getSalesInvoices(org_view);
    case "Yours credit notes":
      if (search) {
        return await getSearchCreditNotes(org_view, true, search);
      }
      return await getCreditNotes(org_view, true);
    case "Client credit notes":
      if (search) {
        return await getSearchCreditNotes(org_view, false, search);
      }
      return await getCreditNotes(org_view, false);
    case "Requests":
      return [];
  }
  if (search) {
    return await getSearchYoursInvoices(org_view, search);
  }

  return await getYoursInvoices(org_view, search);
}

export default async function InvoicesPage({ searchParams }) {
  const current_role = await getRole();
  const userInfo = await getBasicInfo();
  const current_nofitication_qty = await getNotificationCounter();
  const is_org_switch_needed = current_role == "Admin";
  let orgActivated =
    searchParams.isOrg !== undefined ? searchParams.isOrg : false;
  let org_view = getOrgView(current_role, orgActivated === "true");
  let isSearchTrue = searchParams.searchQuery !== undefined;
  let docType = searchParams.docType ? searchParams.docType : "Yours invoices";
  let invoices = isSearchTrue
    ? await getDocuments(docType, org_view, searchParams.searchQuery)
    : await getDocuments(docType, org_view);
  let maxInstanceOnPage = searchParams.pagation ? searchParams.pagation : 10;
  let pageQty = Math.ceil(invoices.length / maxInstanceOnPage);
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
        />
      </section>

      <WholeFooter
        max_instance_on_page={maxInstanceOnPage}
        page_qty={pageQty}
        current_page={currentPage}
      />
    </main>
  );
}
