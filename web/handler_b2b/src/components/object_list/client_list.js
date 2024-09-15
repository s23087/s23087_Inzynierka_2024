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
  const [selectedKeys] = useState([]);
  // Selected bar button
  const [isClicked, setIsClicked] = useState(false);
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  const accessibleParams = new URLSearchParams(params);
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
                selected={selectedKeys.indexOf(value.clientId) !== -1}
                selectQtyAction={() => {
                  selectedKeys.push(value.clientId);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectQtyAction={() => {
                  let index = selectedKeys.indexOf(value.clientId);
                  selectedKeys.splice(index, 1);
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
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          let start = page * pagation - pagation;
          let end = page * pagation;
          Object.values(clients)
            .slice(start, end)
            .forEach((e) => selectedKeys.push(e.clientId));
          setSelectedQty(selectedKeys.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          Object.values(clients).forEach((e) => selectedKeys.push(e.clientId));
          setSelectedQty(selectedKeys.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedKeys.splice(0, selectedKeys.length);
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
