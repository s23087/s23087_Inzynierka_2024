"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import ItemContainer from "../object_container/item_container";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import AddItemOffcanvas from "../offcanvas/create/create_item";
import { useSearchParams } from "next/navigation";
import DeleteObjectWindow from "../windows/delete_object";
import ViewItemOffcanvas from "../offcanvas/view/view_item";
import deleteItem from "@/utils/warehouse/delete_item";
import { useRouter } from "next/navigation";
import ModifyItemOffcanvas from "../offcanvas/modify/modify_item";
import SelectComponent from "../smaller_components/select_compontent";
import getPagationInfo from "@/utils/flexible/get_page_info";
import ProductFilterOffcanvas from "../filter/product_filter_offcanvas";
import DeleteSelectedWindow from "../windows/delete_selected";

function ProductList({
  products,
  orgView,
  currency,
  productStart,
  productEnd,
  filterActive,
  currentSort,
}) {
  // View Item
  const [showViewItem, setShowViewItem] = useState(false);
  const [itemToView, setItemToView] = useState({
    users: [],
    eans: [],
  });
  // Modify Item
  const [showModifyItem, setShowModifyItem] = useState(false);
  const [itemToModify, setItemToModify] = useState({
    users: [],
    eans: [],
  });
  // Delete item
  const [showDeleteItem, setShowDeleteItem] = useState(false);
  const [itemToDelete, setItemToDelete] = useState(null);
  const [isErrorDelete, setIsErrorDelete] = useState(false);
  const [deleteErrorMessage, setDeleteErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddItem, setShowAddItem] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedProducts] = useState([]);
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
      <ProductFilterOffcanvas
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
      {Object.keys(products ?? []).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">
            {products ? "Items not found :/" : "Could not connect to server."}
          </p>
        </Container>
      ) : (
        Object.values(products ?? [])
          .slice(productStart, productEnd)
          .map((value) => {
            return (
              <ItemContainer
                item={value}
                currency={currency}
                is_org={orgView}
                selectQtyAction={() => {
                  selectedProducts.push(value.itemId);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectQtyAction={() => {
                  let index = selectedProducts.indexOf(value.itemId);
                  selectedProducts.splice(index, 1);
                  setSelectedQty(selectedQty - 1);
                }}
                deleteAction={() => {
                  setItemToDelete(value.itemId);
                  setShowDeleteItem(true);
                }}
                viewAction={() => {
                  setItemToView(value);
                  setShowViewItem(true);
                }}
                modifyAction={() => {
                  setItemToModify(value);
                  setShowModifyItem(true);
                }}
                selected={selectedProducts.indexOf(value.itemId) !== -1}
                key={[products, value.itemId]}
              />
            );
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName="item"
        addAction={() => {
          setShowMoreAction(false);
          setShowAddItem(true);
        }}
        selectAllOnPage={() => {
          selectedProducts.splice(0, selectedProducts.length);
          setSelectedQty(0);
          let pagationInfo = getPagationInfo(params);
          Object.values(products ?? [])
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) => selectedProducts.push(e.itemId));
          setSelectedQty(selectedProducts.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedProducts.splice(0, selectedProducts.length);
          setSelectedQty(0);
          Object.values(products ?? []).forEach((e) =>
            selectedProducts.push(e.itemId),
          );
          setSelectedQty(selectedProducts.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedProducts.splice(0, selectedProducts.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
      />
      <AddItemOffcanvas
        showOffcanvas={isShowAddItem}
        hideFunction={() => setShowAddItem(false)}
      />
      <DeleteObjectWindow
        modalShow={showDeleteItem}
        onHideFunction={() => {
          setShowDeleteItem(false);
          setDeleteErrorMessage("");
          setIsErrorDelete(false);
        }}
        instanceName="item"
        instanceId={itemToDelete}
        deleteItemFunc={async () => {
          let result = await deleteItem(itemToDelete);
          if (result.result) {
            setShowDeleteItem(false);
            router.refresh();
          } else {
            setDeleteErrorMessage(result.message);
            setIsErrorDelete(true);
          }
        }}
        isError={isErrorDelete}
        errorMessage={deleteErrorMessage}
      />
      <DeleteSelectedWindow
        modalShow={showDeleteSelected}
        onHideFunction={() => {
          setShowDeleteSelected(false);
          setDeleteSelectedErrorMess("");
          setIsErrorDelete(false);
        }}
        instanceName="item"
        deleteItemFunc={async () => {
          let failures = [];
          for (let index = 0; index < selectedProducts.length; index++) {
            let result = await deleteItem(selectedProducts[index]);
            if (!result.result) {
              failures.push(selectedProducts[index]);
            } else {
              selectedProducts.splice(index, 1);
              setSelectedQty(selectedProducts.length);
            }
          }
          if (failures.length === 0) {
            setShowDeleteSelected(false);
            setDeleteSelectedErrorMess("");
            router.refresh();
          } else {
            setDeleteSelectedErrorMess(
              `Error: Could not delete this items (${failures.join(",")}).`,
            );
            setIsErrorDelete(true);
            router.refresh();
          }
        }}
        isError={isErrorDelete}
        errorMessage={deleteSelectedErrorMess}
      />
      <ViewItemOffcanvas
        showOffcanvas={showViewItem}
        hideFunction={() => setShowViewItem(false)}
        item={itemToView}
        currency={currency}
        isOrg={orgView}
      />
      <ModifyItemOffcanvas
        showOffcanvas={showModifyItem}
        hideFunction={() => setShowModifyItem(false)}
        item={itemToModify}
        curenncy={currency}
        isOrg={orgView}
      />
    </Container>
  );
}

ProductList.propTypes = {
  products: PropTypes.object.isRequired,
  orgView: PropTypes.bool.isRequired,
  currency: PropTypes.string.isRequired,
  productStart: PropTypes.number.isRequired,
  productEnd: PropTypes.number.isRequired,
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default ProductList;
