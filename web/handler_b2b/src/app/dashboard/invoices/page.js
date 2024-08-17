"use client";

import { useState } from "react";
import { Container, Stack } from "react-bootstrap";
import PagePostionBar from "@/components/menu/page_position_bar";
import SearchFilterBar from "@/components/menu/search_filter_bar";
import MenuTemplate from "@/components/menu/menu_template";
import CustomSidebar from "@/components/menu/sidebars/sidebar";
import Navlinks from "@/components/menu/navlinks";
import PagationFooter from "@/components/footer/pagation_footer";
import InvoiceSwitch from "@/components/switches/invoice_switch";
import InvoiceContainer from "@/components/object_container/documents/yours_invoice_container";
import CreditNoteContainer from "@/components/object_container/documents/credit_note_contianter";
import RequestContainer from "@/components/object_container/documents/request_container";

function getDocument(type, document, is_org, selected) {
  switch (type) {
    case "Sales invoices":
      return (
        <InvoiceContainer
          invoice={document}
          is_org={is_org}
          selected={selected}
          is_user_type={false}
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
      invoice={document}
      is_org={is_org}
      selected={selected}
      is_user_type={true}
    />
  );
}

export default function InvoicesPage() {
  const [sidebarShow, setSidebarShow] = useState(false);
  const showSidebar = () => setSidebarShow(true);
  const hideSidebar = () => setSidebarShow(false);
  const current_role = "Admin";
  const current_nofitication_qty = 1;
  const is_org_switch_needed = true;
  const org_view = false;
  const [documentType, setDocumentType] = useState("Yours invoices");
  const changeDoc = (type) => {
    setDocumentType(type);
  };
  const tmp_your_invoice = {
    user: "<<user>>",
    number: "ab-2022-02-01",
    date: "dd.mm.yyyy",
    status: "Paid",
    qty: 3,
    total_value: "200",
    currency_name: "PLN",
    source: "<<organization>>",
    system: "In system",
    due_date: "dd.mm.yyyy",
  };
  const tmp_client_invoice = {
    user: "<<user>>",
    number: "ab-2022-02-01",
    date: "dd.mm.yyyy",
    status: "Unpaid",
    qty: 4,
    total_value: "300",
    currency_name: "PLN",
    buyer: "<<organization>>",
    system: "Requested",
    due_date: "dd.mm.yyyy",
  };
  const tmp_your_credit_note = {
    user: "<<user>>",
    invoice: "ab-2022-02-01",
    date: "dd.mm.yyyy",
    qty: 3,
    total_value: "-200",
    currency_name: "PLN",
    for: "<<organization>>",
    system: "In system",
    due_date: "dd.mm.yyyy",
  };
  const tmp_client_credit_note = {
    user: "<<user>>",
    invoice: "ab-2022-02-01",
    date: "dd.mm.yyyy",
    qty: 4,
    total_value: "-300",
    currency_name: "PLN",
    source: "<<organization>>",
    system: "Not in system",
    due_date: "dd.mm.yyyy",
  };
  const tmp_request = {
    user: "<<user>>",
    document: "ab-2022-02-01",
    date: "dd.mm.yyyy",
    status: "Rejected",
    qty: 3,
    total_value: "-200",
    currency_name: "PLN",
    type: "<<type>>",
    system: "In system",
  };

  return (
    <main className="d-flex flex-column h-100">
      <nav className="fixed-top main-bg">
        <MenuTemplate sidebar_action={showSidebar} user_name="<<User name>>">
          <Stack className="ps-xl-2" direction="horizontal" gap={4}>
            <Container className="mx-auto mx-xl-2 me-xl-5 w-auto">
              <InvoiceSwitch
                type={documentType}
                switch_action={changeDoc}
                is_role_solo={current_role === "Solo"}
              />
            </Container>
            <Stack className="d-none d-xl-flex" direction="horizontal" gap={4}>
              <Navlinks
                role={current_role}
                active_link="Invoices"
                notification_qty={current_nofitication_qty}
                is_sidebar={false}
              />
            </Stack>
          </Stack>
        </MenuTemplate>
        <PagePostionBar
          site_name={documentType}
          with_switch={is_org_switch_needed}
          switch_bool={org_view}
        />
        <SearchFilterBar filter_icon_bool="false" />
        <CustomSidebar
          user_name="<<User name>>"
          org_name="<<Org name>>"
          offcanvasShow={sidebarShow}
          onHideAction={hideSidebar}
        >
          <Navlinks
            role={current_role}
            active_link="Invoices"
            notification_qty={current_nofitication_qty}
            is_sidebar={true}
          />
        </CustomSidebar>
      </nav>

      <section className="h-100">
        <Container className="p-0 middleSectionPlacement" fluid>
          {getDocument("Your invoices", tmp_your_invoice, false, false)}
          {getDocument("Your invoices", tmp_your_invoice, false, true)}
          {getDocument("Your invoices", tmp_your_invoice, true, false)}
          {getDocument("Your invoices", tmp_your_invoice, true, true)}
          {getDocument("Sales invoices", tmp_client_invoice, false, false)}
          {getDocument("Sales invoices", tmp_client_invoice, false, true)}
          {getDocument("Sales invoices", tmp_client_invoice, true, false)}
          {getDocument("Sales invoices", tmp_client_invoice, true, true)}
          {getDocument(
            "Yours credit notes",
            tmp_your_credit_note,
            false,
            false,
          )}
          {getDocument("Yours credit notes", tmp_your_credit_note, false, true)}
          {getDocument("Yours credit notes", tmp_your_credit_note, true, false)}
          {getDocument("Yours credit notes", tmp_your_credit_note, true, true)}
          {getDocument(
            "Client credit notes",
            tmp_client_credit_note,
            false,
            false,
          )}
          {getDocument(
            "Client credit notes",
            tmp_client_credit_note,
            false,
            true,
          )}
          {getDocument(
            "Client credit notes",
            tmp_client_credit_note,
            true,
            false,
          )}
          {getDocument(
            "Client credit notes",
            tmp_client_credit_note,
            true,
            true,
          )}
          {getDocument("Requests", tmp_request, false, false)}
          {getDocument("Requests", tmp_request, false, true)}
          {getDocument("Requests", tmp_request, true, false)}
          {getDocument("Requests", tmp_request, true, true)}
        </Container>
      </section>

      <footer className="fixed-bottom w-100">
        <PagationFooter
          max_instance_on_page={10}
          page_qty={20}
          current_page={1}
        />
      </footer>
    </main>
  );
}
