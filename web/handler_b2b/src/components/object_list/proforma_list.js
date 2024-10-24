"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams, useRouter } from "next/navigation";
import DeleteObjectWindow from "../windows/delete_object";
import SelectComponent from "../smaller_components/select_compontent";
import getPagationInfo from "@/utils/flexible/get_page_info";
import ProformaContainer from "../object_container/proforma_container";
import AddProformaOffcanvas from "../offcanvas/create/create_proforma";
import ViewProformaOffcanvas from "../offcanvas/view/view_proforma";
import deleteProforma from "@/utils/proformas/delete_proforma";
import ModifyProformaOffcanvas from "../offcanvas/modify/modify_proforma";
import ProformaFilterOffcanvas from "../filter/proforma_filter";
import DeleteSelectedWindow from "../windows/delete_selected";

function ProformaList({
  proformas,
  type,
  orgView,
  proformasStart,
  proformasEnd,
  filterActive,
  currentSort,
}) {
  // View proforma
  const [showViewProforma, setShowViewProforma] = useState(false);
  const [proformaToView, setProformaToView] = useState({
    proformaId: 0,
    user: "",
    date: "",
    transport: 0.0,
    clientName: "",
    qty: 0,
    total: 0.0,
  });
  // Modify proforma
  const [showModifyProforma, setShowModifyProforma] = useState(false);
  const [proformaToModify, setProformaToModify] = useState({
    proformaId: 0,
    user: "",
    date: "",
    transport: 0.0,
    clientName: "",
    qty: 0,
    total: 0.0,
  });
  // Delete proforma
  const [showDeleteProforma, setShowDeleteProforma] = useState(false);
  const [proformaToDelete, setProformaToDelete] = useState(null);
  const [isErrorDelete, setIsErrorDelete] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddProforma, setShowAddProforma] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedProforma] = useState([]);
  // Filter
  const [showFilter, setShowFilter] = useState(false);
  // mass action
  const [showDeleteSelected, setShowDeleteSelected] = useState(false);
  const [deleteSelectedErrorMess, setDeleteSelectedErrorMess] = useState("");
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="px-0 middleSectionPlacement position-relative" fluid>
      <ProformaFilterOffcanvas
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
      {Object.keys(proformas ?? []).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">
            {proformas
              ? "Proformas not found :/"
              : "Could not connect to server."}
          </p>
        </Container>
      ) : (
        Object.values(proformas ?? [])
          .slice(proformasStart, proformasEnd)
          .map((value) => {
            return (
              <ProformaContainer
                key={value.proformaId}
                proforma={value}
                is_org={orgView}
                isYourProforma={type === "Yours proformas"}
                selected={selectedProforma.indexOf(value.proformaId) !== -1}
                selectAction={() => {
                  selectedProforma.push(value.proformaId);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectAction={() => {
                  let index = selectedProforma.indexOf(value.proformaId);
                  selectedProforma.splice(index, 1);
                  setSelectedQty(selectedQty - 1);
                }}
                deleteAction={() => {
                  setProformaToDelete(value.proformaId);
                  setShowDeleteProforma(true);
                }}
                viewAction={() => {
                  setProformaToView(value);
                  setShowViewProforma(true);
                }}
                modifyAction={() => {
                  setProformaToModify(value);
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
          Object.values(proformas ?? [])
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) => selectedProforma.push(e.proformaId));
          setSelectedQty(selectedProforma.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedProforma.splice(0, selectedProforma.length);
          setSelectedQty(0);
          Object.values(proformas ?? []).forEach((e) =>
            selectedProforma.push(e.proformaId),
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
          let result = await deleteProforma(
            type === "Yours proformas",
            proformaToDelete,
          );
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
      <DeleteSelectedWindow
        modalShow={showDeleteSelected}
        onHideFunction={() => {
          setShowDeleteSelected(false);
          setDeleteSelectedErrorMess("");
          setIsErrorDelete(false);
        }}
        instanceName="Proforma"
        deleteItemFunc={async () => {
          let failures = [];
          for (let index = 0; index < selectedProforma.length; index++) {
            let result = await deleteProforma(
              type === "Yours proformas",
              selectedProforma[index],
            );
            if (result.error) {
              failures.push(selectedProforma[index]);
            } else {
              selectedProforma.splice(index, 1);
              setSelectedQty(selectedProforma.length);
            }
          }
          if (failures.length === 0) {
            setShowDeleteSelected(false);
            setDeleteSelectedErrorMess("");
            router.refresh();
          } else {
            setDeleteSelectedErrorMess(
              `Error: Could not delete this proformas (${failures.join(",")}).`,
            );
            setIsErrorDelete(true);
            router.refresh();
          }
        }}
        isError={isErrorDelete}
        errorMessage={deleteSelectedErrorMess}
      />
      <ModifyProformaOffcanvas
        showOffcanvas={showModifyProforma}
        hideFunction={() => setShowModifyProforma(false)}
        proforma={proformaToModify}
        isYourProforma={type === "Yours proformas"}
      />
      <ViewProformaOffcanvas
        showOffcanvas={showViewProforma}
        hideFunction={() => setShowViewProforma(false)}
        proforma={proformaToView}
        isYourProforma={type === "Yours proformas"}
        isOrg={orgView}
      />
    </Container>
  );
}

ProformaList.propTypes = {
  proformas: PropTypes.object.isRequired,
  type: PropTypes.string.isRequired, // String that tells which type of proforma is it
  orgView: PropTypes.bool.isRequired,
  proformasStart: PropTypes.number.isRequired,
  proformasEnd: PropTypes.number.isRequired,
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default ProformaList;
