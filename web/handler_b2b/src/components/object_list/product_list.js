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

function ProductList({
  products,
  orgView,
  currency,
  productStart,
  productEnd,
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
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  const [isShowAddItem, setShowAddItem] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedProducts] = useState([]);
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  // Styles
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
      {Object.keys(products).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">Items not found :/</p>
        </Container>
      ) : (
        Object.values(products)
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
                key={value.itemId}
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
          Object.values(products)
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) => selectedProducts.push(e.itemId));
          setSelectedQty(selectedProducts.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedProducts.splice(0, selectedProducts.length);
          setSelectedQty(0);
          Object.values(products).forEach((e) =>
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
        onHideFunction={() => setShowDeleteItem(false)}
        instanceName="item"
        instanceId={itemToDelete}
        deleteItemFunc={async () => {
          let result = await deleteItem(itemToDelete);
          if (result) {
            setShowDeleteItem(false);
            router.refresh();
          } else {
            setIsErrorDelete(true);
          }
        }}
        isError={isErrorDelete}
        errorMessage="Error: Could not delete this item. Check if invoice with this item exist."
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

ProductList.PropTypes = {
  products: PropTypes.object.isRequired,
  orgView: PropTypes.bool.isRequired,
  currency: PropTypes.string.isRequired,
  productStart: PropTypes.number.isRequired,
  productEnd: PropTypes.number.isRequired,
};

export default ProductList;
