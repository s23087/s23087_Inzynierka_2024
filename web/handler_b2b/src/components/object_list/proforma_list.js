"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams } from "next/navigation";
import DeleteObjectWindow from "../windows/delete_object";
import ViewClientOffcanvas from "../offcanvas/view/view_client";
import deleteClient from "@/utils/clients/delete_client";
import { useRouter } from "next/navigation";
import ModifyClientOffcanvas from "../offcanvas/modify/modify_client";
import SelectComponent from "../smaller_components/select_compontent";
import getPagationInfo from "@/utils/flexible/get_page_info";
import ProformaContainer from "../object_container/proforma_container";
import AddProformaOffcanvas from "../offcanvas/create/create_proforma";

function ProformaList({
  proformas,
  type,
  orgView,
  proformasStart,
  proformasEnd,
}) {
  // View proforma
  const [showViewProforma, setShowViewProforma] = useState(false);
  const [proformaToView, setProformaToView] = useState({
    users: [],
  });
  // Modify proforma
  const [showModifyProforma, setShowModifyProforma] = useState(false);
  const [proformaToModify, setClientToProforma] = useState({
    users: [],
  });
  // Delete proforma
  const [showDeleteProforma, setShowDeleteProforma] = useState(false);
  const [proformaToDelete, setItemToProforma] = useState(null);
  const [isErrorDelete, setIsErrorDelete] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddProforma, setShowAddProforma] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedProforma] = useState([]);
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
      {Object.keys(proformas).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">Proformas not found :/</p>
        </Container>
      ) : (
        Object.values(proformas)
          .slice(proformasStart, proformasEnd)
          .map((value) => {
            return (
              <ProformaContainer
                key={value.clientId}
                proforma={value}
                is_org={orgView}
                isYourProforma={type === "Yours proformas"}
                selected={selectedProforma.indexOf(value.clientId) !== -1}
                selectAction={() => {
                  selectedProforma.push(value.clientId);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectAction={() => {
                  let index = selectedProforma.indexOf(value.clientId);
                  selectedProforma.splice(index, 1);
                  setSelectedQty(selectedQty - 1);
                }}
                deleteAction={() => {
                  setItemToProforma(value.clientId);
                  setShowDeleteProforma(true);
                }}
                viewAction={() => {
                  setProformaToView(value);
                  setShowViewProforma(true);
                }}
                modifyAction={() => {
                  setClientToProforma(value);
                  setShowModifyProforma(true);
                }}
              />
            );
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName="proforma"
        addAction={() => {
          setShowMoreAction(false);
          setShowAddProforma(true);
        }}
        selectAllOnPage={() => {
          selectedProforma.splice(0, selectedProforma.length);
          setSelectedQty(0);
          let pagationInfo = getPagationInfo(params);
          Object.values(proformas)
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) => selectedProforma.push(e.clientId));
          setSelectedQty(selectedProforma.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedProforma.splice(0, selectedProforma.length);
          setSelectedQty(0);
          Object.values(proformas).forEach((e) =>
            selectedProforma.push(e.clientId),
          );
          setSelectedQty(selectedProforma.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedProforma.splice(0, selectedProforma.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
      />
      <AddProformaOffcanvas
        showOffcanvas={isShowAddProforma}
        hideFunction={() => setShowAddProforma(false)}
        isYourProforma={type === "Yours proformas"}
      />
      <DeleteObjectWindow
        modalShow={showDeleteProforma}
        onHideFunction={() => {
          setShowDeleteProforma(false);
          setIsErrorDelete(false);
        }}
        instanceName="proforma"
        instanceId={proformaToDelete}
        deleteItemFunc={async () => {
          let result = await deleteClient(proformaToDelete);
          if (!result.error) {
            setShowDeleteProforma(false);
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
        showOffcanvas={showModifyProforma}
        hideFunction={() => setShowModifyProforma(false)}
        client={proformaToModify}
        isOrg={orgView}
      />
      <ViewClientOffcanvas
        showOffcanvas={showViewProforma}
        hideFunction={() => setShowViewProforma(false)}
        client={proformaToView}
        isOrg={orgView}
      />
    </Container>
  );
}

ProformaList.PropTypes = {
  proformas: PropTypes.object.isRequired,
  orgView: PropTypes.bool.isRequired,
  clientsStart: PropTypes.number.isRequired,
  clientsEnd: PropTypes.number.isRequired,
};

export default ProformaList;
