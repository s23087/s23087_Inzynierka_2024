"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams } from "next/navigation";
import RoleContainer from "../object_container/role_container";
import ModifyUserRole from "../windows/modify_user_roles";
import SelectComponent from "../smaller_components/select_component";
import getPaginationInfo from "@/utils/flexible/get_page_info";
import RoleFilterOffcanvas from "../filter/role_filter";
import ModifySelectedUserRole from "../windows/modify_selected_users_roles";

/**
 * Return component that showcase role objects, search bar, filter, more action element and selected element.
 * @component
 * @param {object} props Component props
 * @param {Array<{userId: Number, username: string, surname: string, roleName: string}>} props.roles Array containing role objects.
 * @param {Number} props.rolesStart Starting index of roles subarray.
 * @param {Number} props.rolesEnd Ending index of roles subarray.
 * @param {Array<string>} props.rolesToChoose List of roles.
 * @param {boolean} props.filterActive If filter is activated then true, otherwise false.
 * @param {string} props.currentSort Current value of "sort" query parameter
 * @return {JSX.Element} Container element
 */
function RolesList({
  roles,
  rolesStart,
  rolesEnd,
  rolesToChoose,
  filterActive,
  currentSort,
}) {
  const params = useSearchParams();
  // useState for showing modify user role window
  const [showModifyRole, setShowModifyRole] = useState(false);
  // holder for user soon to be modified
  const [userToModify, setUserToModify] = useState({});
  // useState for showing more action window
  const [showMoreAction, setShowMoreAction] = useState(false);
  // Number of selected objects
  const [selectedQty, setSelectedQty] = useState(0);
  // Selected role keys
  const [selectedRoles] = useState([]);
  // useState for showing filter offcanvas
  const [showFilter, setShowFilter] = useState(false);
  // useState for showing modify selected user role window
  const [showModifySelectedRole, setShowModifySelectedRole] = useState(false);
  // Styles
  const containerMargin = {
    height: "67px",
  };
  return (
    <Container className="px-0 middleSectionPlacement position-relative" fluid>
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
          let paginationInfo = getPaginationInfo(params);
          Object.values(roles ?? [])
            .slice(paginationInfo.start, paginationInfo.end)
            .forEach((e) => selectedRoles.push(e.userId));
          setSelectedQty(selectedRoles.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedRoles.splice(0, selectedRoles.length);
          setSelectedQty(0);
          Object.values(roles ?? []).forEach((e) =>
            selectedRoles.push(e.userId),
          );
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

RolesList.propTypes = {
  roles: PropTypes.object.isRequired,
  rolesStart: PropTypes.number.isRequired,
  rolesEnd: PropTypes.number.isRequired,
  rolesToChoose: PropTypes.object.isRequired,
  filterActive: PropTypes.bool.isRequired,
  currentSort: PropTypes.string.isRequired,
};

export default RolesList;
