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
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  // Styles
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="p-0 middleSectionPlacement position-relative" fluid>
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
      <SelectComponent selectedQty={selectedQty} additonalMargin={true} />
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
                  selectedItems.filter(
                    (e) => e.itemId === value.itemId && e.orgId === value.orgId,
                  ).length !== 0
                }
                selectAction={() => {
                  selectedItems.push({
                    itemId: value.itemId,
                    orgId: value.orgId,
                  });
                  setSelectedQty(selectedQty + 1);
                }}
                unselectAction={() => {
                  let index = selectedItems.indexOf({
                    itemId: value.itemId,
                    orgId: value.orgId,
                  });
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
          Object.values(items)
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) =>
              selectedItems.push({ itemId: e.itemId, orgId: e.orgId }),
            );
          setSelectedQty(selectedItems.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedItems.splice(0, selectedItems.length);
          setSelectedQty(0);
          Object.values(items).forEach((e) =>
            selectedItems.push({ itemId: e.itemId, orgId: e.orgId }),
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
    </Container>
  );
}

OutsideItemList.PropTypes = {
  items: PropTypes.object.isRequired,
  itemsStart: PropTypes.number.isRequired,
  itemsEnd: PropTypes.number.isRequired,
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default OutsideItemList;
