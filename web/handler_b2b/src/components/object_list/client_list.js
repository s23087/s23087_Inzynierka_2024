"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import ClientContainer from "../object_container/clients_container";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import AddClientOffcanvas from "../offcanvas/create/create_client";
import DeleteObjectWindow from "../windows/delete_object";
import ViewClientOffcanvas from "../offcanvas/view/view_client";
import deleteClient from "@/utils/clients/delete_client";
import { useRouter, useSearchParams } from "next/navigation";
import ModifyClientOffcanvas from "../offcanvas/modify/modify_client";
import SelectComponent from "../smaller_components/select_component";
import getPaginationInfo from "@/utils/flexible/get_page_info";
import ClientFilterOffcanvas from "../filter/client_filter";
import DeleteSelectedWindow from "../windows/delete_selected";

/**
 * Return component that showcase client objects, search bar, filter, more action element and selected element.
 * @component
 * @param {object} props Component props
 * @param {Array<{clientId: Number, clientName: string, street: string, city: string, postal: string, nip: Number|undefined, country: string}>} props.clients Array containing client objects.
 * @param {boolean} props.orgView True if org view is enabled.
 * @param {Number} props.clientsStart Starting index of clients subarray.
 * @param {Number} props.clientsEnd Ending index of clients subarray.
 * @param {boolean} props.filterActive If filter is activated then true, otherwise false.
 * @param {string} props.currentSort Current value of "sort" query parameter
 * @return {JSX.Element} Container element
 */
function ClientsList({
  clients,
  orgView,
  clientsStart,
  clientsEnd,
  filterActive,
  currentSort,
}) {
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  // View client
  const [showViewClient, setShowViewClient] = useState(false); // useState for showing view offcanvas
  const [clientToView, setClientToView] = useState({
    users: [],
  }); // holder of object chosen to view
  // Modify client
  const [showModifyClient, setShowModifyClient] = useState(false); // useState for showing modify offcanvas
  const [clientToModify, setClientToModify] = useState({
    users: [],
  }); // holder of object chosen to modify
  // Delete client
  const [showDeleteClient, setShowDeleteClient] = useState(false); // useState for showing delete window
  const [clientToDelete, setItemToDelete] = useState(null); // holder of object chosen to delete
  const [isErrorDelete, setIsErrorDelete] = useState(false); // true if delete action failed
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false); // useState for showing more action window
  const [isShowAddClient, setShowAddClient] = useState(false); // useState for showing create offcanvas
  // Selected
  const [selectedQty, setSelectedQty] = useState(0); // Number of selected objects
  const [selectedClients] = useState([]); // Selected proforma keys
  // Filter
  const [showFilter, setShowFilter] = useState(false); // useState for showing filter offcanvas
  // mass action
  const [showDeleteSelected, setShowDeleteSelected] = useState(false); // useState for showing mass action delete window
  const [deleteSelectedErrorMess, setDeleteSelectedErrorMess] = useState(""); // error message of mass delete action
  // Styles
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="px-0 middleSectionPlacement position-relative" fluid>
      <ClientFilterOffcanvas
        showOffcanvas={showFilter}
        hideFunction={() => setShowFilter(false)}
        currentSort={currentSort}
        currentDirection={
          currentSort.startsWith("A") || currentSort.startsWith(".")
        }
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
      {Object.keys(clients ?? []).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">
            {clients ? "Clients not found :/" : "Could not connect to server."}
          </p>
        </Container>
      ) : (
        Object.values(clients ?? [])
          .slice(clientsStart, clientsEnd)
          .map((value) => {
            return (
              <ClientContainer
                key={value.clientId}
                client={value}
                is_org={orgView}
                selected={selectedClients.indexOf(value.clientId) !== -1}
                selectAction={() => {
                  selectedClients.push(value.clientId);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectAction={() => {
                  let index = selectedClients.indexOf(value.clientId);
                  selectedClients.splice(index, 1);
                  setSelectedQty(selectedQty - 1);
                }}
                deleteAction={() => {
                  setItemToDelete(value.clientId);
                  setShowDeleteClient(true);
                }}
                viewAction={() => {
                  setClientToView(value);
                  setShowViewClient(true);
                }}
                modifyAction={() => {
                  setClientToModify(value);
                  setShowModifyClient(true);
                }}
              />
            );
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName="client"
        addAction={() => {
          setShowMoreAction(false);
          setShowAddClient(true);
        }}
        selectAllOnPage={() => {
          selectedClients.splice(0, selectedClients.length);
          setSelectedQty(0);
          let paginationInfo = getPaginationInfo(params);
          Object.values(clients ?? [])
            .slice(paginationInfo.start, paginationInfo.end)
            .forEach((e) => selectedClients.push(e.clientId));
          setSelectedQty(selectedClients.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedClients.splice(0, selectedClients.length);
          setSelectedQty(0);
          Object.values(clients ?? []).forEach((e) =>
            selectedClients.push(e.clientId),
          );
          setSelectedQty(selectedClients.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedClients.splice(0, selectedClients.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
      />
      <AddClientOffcanvas
        showOffcanvas={isShowAddClient}
        hideFunction={() => setShowAddClient(false)}
      />
      <DeleteObjectWindow
        modalShow={showDeleteClient}
        onHideFunction={() => {
          setShowDeleteClient(false);
          setIsErrorDelete(false);
        }}
        instanceName="client"
        instanceId={clientToDelete}
        deleteItemFunc={async () => {
          let result = await deleteClient(clientToDelete);
          if (!result.error) {
            setShowDeleteClient(false);
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
        instanceName="client"
        deleteItemFunc={async () => {
          let failures = [];
          for (let index = 0; index < selectedClients.length; index++) {
            let result = await deleteClient(selectedClients[index]);
            if (result.error) {
              failures.push(selectedClients[index]);
            } else {
              selectedClients.splice(index, 1);
              setSelectedQty(selectedClients.length);
            }
          }
          if (failures.length === 0) {
            setShowDeleteSelected(false);
            setDeleteSelectedErrorMess("");
            router.refresh();
          } else {
            setDeleteSelectedErrorMess(
              `Error: Could not delete this clients (${failures.join(",")}).`,
            );
            setIsErrorDelete(true);
            router.refresh();
          }
        }}
        isError={isErrorDelete}
        errorMessage={deleteSelectedErrorMess}
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

ClientsList.propTypes = {
  clients: PropTypes.object.isRequired,
  orgView: PropTypes.bool.isRequired,
  clientsStart: PropTypes.number.isRequired,
  clientsEnd: PropTypes.number.isRequired,
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default ClientsList;
