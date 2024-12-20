"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams, useRouter } from "next/navigation";
import DeleteObjectWindow from "../windows/delete_object";
import SelectComponent from "../smaller_components/select_component";
import getPaginationInfo from "@/utils/flexible/get_page_info";
import DeliveryContainer from "../object_container/delivery_container";
import AddDeliveryOffcanvas from "../offcanvas/create/create_delivery";
import deleteDelivery from "@/utils/deliveries/delete_delivery";
import ViewDeliveryOffcanvas from "../offcanvas/view/view_delivery";
import ModifyDeliveryOffcanvas from "../offcanvas/modify/modify_delivery";
import DeliveryFilterOffcanvas from "../filter/delivery_filter";
import DeleteSelectedWindow from "../windows/delete_selected";

/**
 * Return component that showcase delivery objects, search bar, filter, more action element and selected element.
 * @component
 * @param {object} props Component props
 * @param {Array<{user: string, deliveryId: Number, status: string, waybill: Array<string>, deliveryCompany: string, estimated: string, proforma: string, clientName: string, delivered: string}>} props.deliveries Array containing delivery objects.
 * @param {string} props.type Name of chosen type of delivery.
 * @param {boolean} props.orgView True if org view is enabled.
 * @param {Number} props.deliveriesStart Starting index of deliveries subarray.
 * @param {Number} props.deliveriesEnd Ending index of deliveries subarray.
 * @param {boolean} props.filterActive If filter is activated then true, otherwise false.
 * @param {string} props.currentSort Current value of "sort" query parameter
 * @return {JSX.Element} Container element
 */
function DeliveryList({
  deliveries,
  type,
  orgView,
  deliveriesStart,
  deliveriesEnd,
  filterActive,
  currentSort,
}) {
  // Nav
  const router = useRouter();
  const params = useSearchParams();
  // View delivery
  const [showViewDelivery, setShowViewDelivery] = useState(false); // useState for showing view offcanvas
  const [deliveryToView, setDeliveryToView] = useState({
    deliveryId: 0,
    user: "",
    proformaNumber: "",
    deliveryCompany: "",
    estimated: "",
    delivered: "",
    status: "",
    waybill: [],
  }); // holder of object chosen to view
  // Modify delivery
  const [showModifyDelivery, setShowModifyDelivery] = useState(false); // useState for showing modify offcanvas
  const [deliveryToModify, setProformaToDelivery] = useState({
    deliveryId: 0,
    user: "",
    date: "",
    transport: 0.0,
    clientName: "",
    qty: 0,
    total: 0.0,
  }); // holder of object chosen to modify
  // Delete delivery
  const [showDeleteDelivery, setShowDeleteDelivery] = useState(false); // useState for showing delete window
  const [deliveryToDelete, setDeliveryToDelete] = useState(null); // holder of object chosen to delete
  const [isErrorDelete, setIsErrorDelete] = useState(false); // true if delete action failed
  const [errorMessage, setErrorMessage] = useState("");
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false); // useState for showing more action window
  const [isShowAddDelivery, setShowAddDelivery] = useState(false); // useState for showing create offcanvas
  // Selected
  const [selectedQty, setSelectedQty] = useState(0); // Number of selected objects
  const [selectedDelivery] = useState([]); // Selected proforma keys
  // Filter
  const [showFilter, setShowFilter] = useState(false); // useState for showing filter offcanvas
  // mass action
  const [showDeleteSelected, setShowDeleteSelected] = useState(false); // useState for showing mass action delete window
  const [deleteSelectedErrorMess, setDeleteSelectedErrorMess] = useState(""); // error message of mass delete action
  // styles
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="px-0 middleSectionPlacement position-relative" fluid>
      <DeliveryFilterOffcanvas
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
      {Object.keys(deliveries ?? []).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">
            {deliveries
              ? "Deliveries not found :/"
              : "Could not connect to server."}
          </p>
        </Container>
      ) : (
        Object.values(deliveries ?? [])
          .slice(deliveriesStart, deliveriesEnd)
          .map((value) => {
            return (
              <DeliveryContainer
                key={value.deliveryId}
                delivery={value}
                is_org={orgView}
                isDeliveryToUser={type === "Deliveries to user"}
                selected={selectedDelivery.indexOf(value.deliveryId) !== -1}
                selectAction={() => {
                  selectedDelivery.push(value.deliveryId);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectAction={() => {
                  let index = selectedDelivery.indexOf(value.deliveryId);
                  selectedDelivery.splice(index, 1);
                  setSelectedQty(selectedQty - 1);
                }}
                deleteAction={() => {
                  setDeliveryToDelete(value.deliveryId);
                  setShowDeleteDelivery(true);
                }}
                viewAction={() => {
                  setDeliveryToView(value);
                  setShowViewDelivery(true);
                }}
                modifyAction={() => {
                  setProformaToDelivery(value);
                  setShowModifyDelivery(true);
                }}
              />
            );
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName="instance"
        addAction={() => {
          setShowMoreAction(false);
          setShowAddDelivery(true);
        }}
        selectAllOnPage={() => {
          selectedDelivery.splice(0, selectedDelivery.length);
          setSelectedQty(0);
          let paginationInfo = getPaginationInfo(params);
          Object.values(deliveries ?? [])
            .slice(paginationInfo.start, paginationInfo.end)
            .forEach((e) => selectedDelivery.push(e.deliveryId));
          setSelectedQty(selectedDelivery.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedDelivery.splice(0, selectedDelivery.length);
          setSelectedQty(0);
          Object.values(deliveries ?? []).forEach((e) =>
            selectedDelivery.push(e.deliveryId),
          );
          setSelectedQty(selectedDelivery.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedDelivery.splice(0, selectedDelivery.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
      />
      <AddDeliveryOffcanvas
        showOffcanvas={isShowAddDelivery}
        hideFunction={() => setShowAddDelivery(false)}
        isDeliveryToUser={type === "Deliveries to user"}
      />
      <DeleteObjectWindow
        modalShow={showDeleteDelivery}
        onHideFunction={() => {
          setShowDeleteDelivery(false);
          setIsErrorDelete(false);
        }}
        instanceName="delivery"
        instanceId={deliveryToDelete}
        deleteItemFunc={async () => {
          let result = await deleteDelivery(
            type === "Deliveries to user",
            deliveryToDelete,
          );
          if (!result.error) {
            setShowDeleteDelivery(false);
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
        instanceName="delivery"
        deleteItemFunc={async () => {
          let failures = [];
          for (let index = 0; index < selectedDelivery.length; index++) {
            let result = await deleteDelivery(
              type === "Deliveries to user",
              selectedDelivery[index],
            );
            if (result.error) {
              failures.push(selectedDelivery[index]);
            } else {
              selectedDelivery.splice(index, 1);
              setSelectedQty(selectedDelivery.length);
            }
          }
          if (failures.length === 0) {
            setShowDeleteSelected(false);
            setDeleteSelectedErrorMess("");
            router.refresh();
          } else {
            setDeleteSelectedErrorMess(
              `Error: Could not delete this deliveries (${failures.join(",")}).`,
            );
            setIsErrorDelete(true);
            router.refresh();
          }
        }}
        isError={isErrorDelete}
        errorMessage={deleteSelectedErrorMess}
      />
      <ModifyDeliveryOffcanvas
        showOffcanvas={showModifyDelivery}
        hideFunction={() => setShowModifyDelivery(false)}
        delivery={deliveryToModify}
        isDeliveryToUser={type === "Deliveries to user"}
      />
      <ViewDeliveryOffcanvas
        showOffcanvas={showViewDelivery}
        hideFunction={() => setShowViewDelivery(false)}
        delivery={deliveryToView}
        isOrg={orgView}
        isDeliveryToUser={type === "Deliveries to user"}
      />
    </Container>
  );
}

DeliveryList.propTypes = {
  deliveries: PropTypes.object.isRequired,
  type: PropTypes.string.isRequired, // String that tells which delivery type is it
  orgView: PropTypes.bool.isRequired,
  deliveriesStart: PropTypes.number.isRequired,
  deliveriesEnd: PropTypes.number.isRequired,
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default DeliveryList;
