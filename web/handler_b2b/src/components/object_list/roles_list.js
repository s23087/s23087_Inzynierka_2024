"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams } from "next/navigation";
import RoleContainer from "../object_container/role_container";
import ModifyUserRole from "../windows/modify_user_roles";
import SelectComponent from "../smaller_components/select_compontent";
import getPagationInfo from "@/utils/flexible/get_page_info";
import RoleFilterOffcanvas from "../filter/role_filter";
import ModifySelectedUserRole from "../windows/modify_selected_users_roles";

function RolesList({
  roles,
  rolesStart,
  rolesEnd,
  rolesToChoose,
  filterActive,
  currentSort,
}) {
  // Modify role
  const [showModifyRole, setShowModifyRole] = useState(false);
  const [userToModify, setUserToModify] = useState({});
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedRoles] = useState([]);
  // Filter
  const [showFilter, setShowFilter] = useState(false);
  // more action
  const [showModifySelectedRole, setShowModifySelectedRole] = useState(false);
  // Nav
  const params = useSearchParams();
  // Styles
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="p-0 middleSectionPlacement position-relative" fluid>
      <RoleFilterOffcanvas
        showOffcanvas={showFilter}
        hideFunction={() => setShowFilter(false)}
        currentSort={currentSort}
        currentDirection={
          currentSort.startsWith("A") || currentSort.startsWith(".")
        }
        rolesToChoose={rolesToChoose ?? []}
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
        actionOneName="Change role"
        actionOne={() => setShowModifySelectedRole(true)}
      />
      <Container style={selectedQty > 0 ? containerMargin : null}></Container>
      {Object.keys(roles ?? []).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">
            {roles ? "Users not found :/" : "Could not connect to server."}
          </p>
        </Container>
      ) : (
        Object.values(roles ?? [])
          .slice(rolesStart, rolesEnd)
          .map((value) => {
            return (
              <RoleContainer
                key={value.userId}
                role={value}
                selected={selectedRoles.indexOf(value.userId) !== -1}
                selectAction={() => {
                  selectedRoles.push(value.userId);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectAction={() => {
                  let index = selectedRoles.indexOf(value.userId);
                  selectedRoles.splice(index, 1);
                  setSelectedQty(selectedQty - 1);
                }}
                modifyAction={() => {
                  setUserToModify(value);
                  setShowModifyRole(true);
                }}
              />
            );
          })
      )}
      <MoreActionWindow
        modalShow={showMoreAction}
        onHideFunction={() => setShowMoreAction(false)}
        instanceName="role"
        selectAllOnPage={() => {
          selectedRoles.splice(0, selectedRoles.length);
          setSelectedQty(0);
          let pagationInfo = getPagationInfo(params);
          Object.values(roles)
            .slice(pagationInfo.start, pagationInfo.end)
            .forEach((e) => selectedRoles.push(e.userId));
          setSelectedQty(selectedRoles.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedRoles.splice(0, selectedRoles.length);
          setSelectedQty(0);
          Object.values(roles).forEach((e) => selectedRoles.push(e.userId));
          setSelectedQty(selectedRoles.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedRoles.splice(0, selectedRoles.length);
          setSelectedQty(0);
          setShowMoreAction(false);
        }}
        withAdd={false}
      />
      <ModifyUserRole
        modalShow={showModifyRole}
        onHideFunction={() => setShowModifyRole(false)}
        roleList={rolesToChoose}
        user={userToModify}
      />
      <ModifySelectedUserRole
        modalShow={showModifySelectedRole}
        onHideFunction={() => setShowModifySelectedRole(false)}
        roleList={rolesToChoose}
        users={selectedRoles}
      />
    </Container>
  );
}

RolesList.PropTypes = {
  products: PropTypes.object.isRequired,
  orgView: PropTypes.bool.isRequired,
  productStart: PropTypes.number.isRequired,
  productEnd: PropTypes.number.isRequired,
  rolesToChoose: PropTypes.object.isRequired,
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default RolesList;
