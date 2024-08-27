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
import ItemContainer from "../object_container/item_container";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import AddItemOffcanvas from "../offcanvas/create/create_item";
import { useSearchParams } from "next/navigation";
import DeleteItemWindow from "../windows/delete_item";
import ViewItemOffcanvas from "../offcanvas/view/view_item";
import deleteItem from "@/utils/warehouse/delete_item";
import { useRouter } from "next/navigation";
import ModifyItemOffcanvas from "../offcanvas/modify/modify_item";

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
    eans: [],
  });
  // Modify Item
  const [showModifyItem, setShowModifyItem] = useState(false);
  const [itemToModify, setItemToModify] = useState({
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
                  selectedKeys.push(value.itemId);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectQtyAction={() => {
                  let index = selectedKeys.indexOf(value.itemId);
                  selectedKeys.splice(index, 1);
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
                selected={selectedKeys.indexOf(value.itemId) !== -1}
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
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          let start = page * pagation - pagation;
          let end = page * pagation;
          Object.values(products)
            .slice(start, end)
            .forEach((e) => selectedKeys.push(e.itemId));
          setSelectedQty(selectedKeys.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          Object.values(products).forEach((e) => selectedKeys.push(e.itemId));
          setSelectedQty(selectedKeys.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
      />
      <AddItemOffcanvas
        showOffcanvas={isShowAddItem}
        hideFunction={() => setShowAddItem(false)}
      />
      <DeleteItemWindow
        modalShow={showDeleteItem}
        onHideFunction={() => setShowDeleteItem(false)}
        instanceName="item"
        instanceId={itemToDelete}
        deleteItemFunc={async () => {
          let result = await deleteItem(itemToDelete);
          console.log(result);
          if (result) {
            setShowDeleteItem(false);
            router.refresh();
          } else {
            setIsErrorDelete(true);
          }
        }}
        isError={isErrorDelete}
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
