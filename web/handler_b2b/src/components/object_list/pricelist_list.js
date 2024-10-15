"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams } from "next/navigation";
import DeleteObjectWindow from "../windows/delete_object";
import { useRouter } from "next/navigation";
import SelectComponent from "../smaller_components/select_compontent";
import getPagationInfo from "@/utils/flexible/get_page_info";
import PricelistContainer from "../object_container/pricelist_container";
import AddPricelistOffcanvas from "../offcanvas/create/create_pricelist";
import deletePricelist from "@/utils/pricelist/delete_pricelist";
import ViewPricelistOffcanvas from "../offcanvas/view/view_pricelist";
import ModifyPricelistOffcanvas from "../offcanvas/modify/modify_pricelist";
import PricelistFilterOffcanvas from "../filter/pricelist_filter";
import DeleteSelectedWindow from "../windows/delete_selected";

function PricelistList({
  pricelist,
  pricelistStart,
  pricelistEnd,
  filterActive,
  currentSort,
}) {
  // View pricelist
  const [showViewPricelist, setShowViewPricelist] = useState(false);
  const [pricelistToView, setPricelistToView] = useState({
    created: "",
    modified: "",
    name: "",
    status: "",
    totalItems: 0,
    currency: "",
  });
  // Modify pricelist
  const [showModifyPricelist, setShowModifyPricelist] = useState(false);
  const [pricelistToModify, setPricelistToModify] = useState({
    created: "",
    modified: "",
    name: "",
    status: "",
    totalItems: 0,
    currency: "",
    path: "",
  });
  // Delete pricelist
  const [showDeletePricelist, setShowDeletePricelist] = useState(false);
  const [pricelistToDelete, setItemToDelete] = useState([0, ""]);
  const [isErrorDelete, setIsErrorDelete] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddPricelist, setShowAddPricelist] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedPricelist] = useState([]);
  // Filter
  const [showFilter, setShowFilter] = useState(false);
  // mass action
  const [showDeleteAll, setDeleteAll] = useState(false);
  const [deleteAllErrorMessage, setDeleteAllErrorMessage] = useState("");
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="px-0 middleSectionPlacement position-relative" fluid>
      <PricelistFilterOffcanvas
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
        actionOne={() => setDeleteAll(true)}
      />
      <Container style={selectedQty > 0 ? containerMargin : null}></Container>
      {Object.keys(pricelist ?? []).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">
            {pricelist === null
              ? "Could not download pricelists"
              : "Pricelist not found :/"}
          </p>
        </Container>
      ) : (
        Object.values(pricelist ?? [])
          .slice(pricelistStart, pricelistEnd)
          .map((value) => {
            return (
              <PricelistContainer
                key={value.pricelistId}
                pricelist={value}
                selected={
                  selectedPricelist.findIndex(
                    (e) => e[0] === value.pricelistId,
                  ) !== -1
                }
                selectAction={() => {
                  selectedPricelist.push([value.pricelistId, value.path]);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectAction={() => {
                  let index = selectedPricelist.findIndex(
                    (e) => e[0] === value.pricelistId,
                  );
                  selectedPricelist.splice(index, 1);
                  setSelectedQty(selectedQty - 1);
                }}
                deleteAction={() => {
                  setItemToDelete([value.pricelistId, value.path]);
                  setShowDeletePricelist(true);
                }}
                viewAction={() => {
                  setPricelistToView(value);
                  setShowViewPricelist(true);
                }}
                modifyAction={() => {
                  setPricelistToModify(value);
                  setShowModifyPricelist(true);
                }}
              />
            );
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName="pricelist"
        addAction={() => {
          setShowMoreAction(false);
          setShowAddPricelist(true);
        }}
        selectAllOnPage={() => {
          selectedPricelist.splice(0, selectedPricelist.length);
          setSelectedQty(0);
          let pagationInfo = getPagationInfo(params);
          Object.values(pricelist ?? [])
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) => selectedPricelist.push([e.pricelistId, e.path]));
          setSelectedQty(selectedPricelist.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedPricelist.splice(0, selectedPricelist.length);
          setSelectedQty(0);
          Object.values(pricelist ?? []).forEach((e) =>
            selectedPricelist.push([e.pricelistId, e.path]),
          );
          setSelectedQty(selectedPricelist.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedPricelist.splice(0, selectedPricelist.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
      />
      <AddPricelistOffcanvas
        showOffcanvas={isShowAddPricelist}
        hideFunction={() => setShowAddPricelist(false)}
      />
      <DeleteObjectWindow
        modalShow={showDeletePricelist}
        onHideFunction={() => {
          setShowDeletePricelist(false);
          setIsErrorDelete(false);
        }}
        instanceName="pricelist"
        instanceId={pricelistToDelete[0]}
        deleteItemFunc={async () => {
          let result = await deletePricelist(
            pricelistToDelete[0],
            pricelistToDelete[1],
          );
          if (!result.error) {
            setShowDeletePricelist(false);
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
        modalShow={showDeleteAll}
        onHideFunction={() => {
          setDeleteAll(false);
          setDeleteAllErrorMessage("");
          setIsErrorDelete(false);
        }}
        instanceName="pricelist"
        deleteItemFunc={async () => {
          let failures = [];
          for (let index = 0; index < selectedPricelist.length; index++) {
            let result = await deletePricelist(
              selectedPricelist[index][0],
              selectedPricelist[index][1],
            );
            if (result.error) {
              failures.push(selectedPricelist[index][0]);
            } else {
              selectedPricelist.splice(index, 1);
              setSelectedQty(selectedPricelist.length);
            }
          }
          if (failures.length === 0) {
            setDeleteAll(false);
            setDeleteAllErrorMessage("");
            router.refresh();
          } else {
            setDeleteAllErrorMessage(
              `Error: Could not delete this pricelists (${failures.join(",")}).`,
            );
            setIsErrorDelete(true);
            router.refresh();
          }
        }}
        isError={isErrorDelete}
        errorMessage={deleteAllErrorMessage}
      />
      <ModifyPricelistOffcanvas
        showOffcanvas={showModifyPricelist}
        hideFunction={() => setShowModifyPricelist(false)}
        pricelist={pricelistToModify}
      />
      <ViewPricelistOffcanvas
        showOffcanvas={showViewPricelist}
        hideFunction={() => setShowViewPricelist(false)}
        pricelist={pricelistToView}
      />
    </Container>
  );
}

PricelistList.PropTypes = {
  pricelist: PropTypes.object.isRequired,
  pricelistStart: PropTypes.number.isRequired,
  pricelistEnd: PropTypes.number.isRequired,
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default PricelistList;
