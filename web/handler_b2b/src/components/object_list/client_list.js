"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import ClientContainer from "../object_container/clients_container";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import AddClientOffcanvas from "../offcanvas/create/create_client";
import { useSearchParams } from "next/navigation";
import DeleteObjectWindow from "../windows/delete_object";
import ViewClientOffcanvas from "../offcanvas/view/view_client";
import deleteClient from "@/utils/clients/delete_client";
import { useRouter } from "next/navigation";
import ModifyClientOffcanvas from "../offcanvas/modify/modify_client";
import SelectComponent from "../smaller_components/select_compontent";
import getPagationInfo from "@/utils/flexible/get_page_info";

function ClientsList({ clients, orgView, clientsStart, clientsEnd }) {
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
  const [showDeleteClient, setShowDeleteclient] = useState(false);
  const [clientToDelete, setItemToDelete] = useState(null);
  const [isErrorDelete, setIsErrorDelete] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddClient, setShowAddClient] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedClients] = useState([]);
  // Nav
  const router = useRouter();
  const params = useSearchParams();
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
      {Object.keys(clients).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">Clients not found :/</p>
        </Container>
      ) : (
        Object.values(clients)
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
                  setShowDeleteclient(true);
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
          let pagationInfo = getPagationInfo(params);
          Object.values(clients)
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) => selectedClients.push(e.clientId));
          setSelectedQty(selectedClients.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedClients.splice(0, selectedClients.length);
          setSelectedQty(0);
          Object.values(clients).forEach((e) =>
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
          setShowDeleteclient(false);
          setIsErrorDelete(false);
        }}
        instanceName="client"
        instanceId={clientToDelete}
        deleteItemFunc={async () => {
          let result = await deleteClient(clientToDelete);
          if (!result.error) {
            setShowDeleteclient(false);
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

ClientsList.PropTypes = {
  clients: PropTypes.object.isRequired,
  orgView: PropTypes.bool.isRequired,
  clientsStart: PropTypes.number.isRequired,
  clientsEnd: PropTypes.number.isRequired,
};

export default ClientsList;
