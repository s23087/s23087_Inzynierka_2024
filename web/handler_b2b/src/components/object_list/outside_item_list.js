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
import AbstractItemContainer from "../object_container/abstrac_item_container";
import AddOutsideItemsOffcanvas from "../offcanvas/create/create_outside_item";
import deleteOutsideItem from "@/utils/outside_items/delete_outside_item";
import OutsideItemsFilterOffcanvas from "../filter/outside_items_filter";
import DeleteSelectedWindow from "../windows/delete_selected";

function OutsideItemList({
  items,
  itemsStart,
  itemsEnd,
  filterActive,
  currentSort,
}) {
  // Delete item
  const [showDeleteItem, setShowDeleteItem] = useState(false);
  const [itemToDelete, setItemToDelete] = useState(null);
  const [isErrorDelete, setIsErrorDelete] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddItems, setShowAddItems] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedItems] = useState([]);
  // Filter
  const [showFilter, setShowFilter] = useState(false);
  // mass action
  const [showDeleteSelected, setShowDeleteSelected] = useState(false);
  const [deleteSelectedErrorMess, setDeleteSelectedErrorMess] = useState("");
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  // Styles
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="px-0 middleSectionPlacement position-relative" fluid>
      <OutsideItemsFilterOffcanvas
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
        additonalMargin={true}
        actionOneName="Delete selected"
        actionOne={() => setShowDeleteSelected(true)}
      />
      <Container style={selectedQty > 0 ? containerMargin : null}></Container>
      {Object.keys(items ?? []).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">{"Items not found :/"}</p>
        </Container>
      ) : (
        Object.values(items ?? [])
          .slice(itemsStart, itemsEnd)
          .map((value) => {
            return (
              <AbstractItemContainer
                key={[value.itemId, value.orgId]}
                abstract_item={value}
                selected={
                  selectedItems.findIndex(
                    (e) => e[0] === value.itemId && e[1] === value.orgId,
                  ) !== -1
                }
                selectAction={() => {
                  selectedItems.push([value.itemId, value.orgId]);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectAction={() => {
                  let index = selectedItems.findIndex(
                    (e) => e[0] === value.itemId && e[1] === value.orgId,
                  );
                  selectedItems.splice(index, 1);
                  setSelectedQty(selectedQty - 1);
                }}
                deleteAction={() => {
                  setItemToDelete([value.itemId, value.orgId]);
                  setShowDeleteItem(true);
                }}
              />
            );
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName="outside item"
        addAction={() => {
          setShowMoreAction(false);
          setShowAddItems(true);
        }}
        selectAllOnPage={() => {
          selectedItems.splice(0, selectedItems.length);
          setSelectedQty(0);
          let pagationInfo = getPagationInfo(params);
          Object.values(items ?? [])
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) => selectedItems.push([e.itemId, e.orgId]));
          setSelectedQty(selectedItems.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedItems.splice(0, selectedItems.length);
          setSelectedQty(0);
          Object.values(items ?? []).forEach((e) =>
            selectedItems.push([e.itemId, e.orgId]),
          );
          setSelectedQty(selectedItems.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedItems.splice(0, selectedItems.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
      />
      <AddOutsideItemsOffcanvas
        showOffcanvas={isShowAddItems}
        hideFunction={() => setShowAddItems(false)}
      />
      <DeleteObjectWindow
        modalShow={showDeleteItem}
        onHideFunction={() => {
          setShowDeleteItem(false);
          setIsErrorDelete(false);
        }}
        instanceName="outside item"
        instanceId={itemToDelete}
        deleteItemFunc={async () => {
          let result = await deleteOutsideItem(
            itemToDelete[0],
            itemToDelete[1],
          );
          if (!result.error) {
            setShowDeleteItem(false);
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
        instanceName="outside item"
        deleteItemFunc={async () => {
          let failures = [];
          for (let index = 0; index < selectedItems.length; index++) {
            let result = await deleteOutsideItem(
              selectedItems[index][0],
              selectedItems[index][1],
            );
            if (result.error) {
              failures.push(selectedItems[index]);
            } else {
              selectedItems.splice(index, 1);
              setSelectedQty(selectedItems.length);
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
    </Container>
  );
}

OutsideItemList.propTypes = {
  items: PropTypes.object.isRequired,
  itemsStart: PropTypes.number.isRequired,
  itemsEnd: PropTypes.number.isRequired,
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default OutsideItemList;
