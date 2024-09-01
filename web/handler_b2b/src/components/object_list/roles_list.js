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
import { useState } from "react";
import SearchFilterBar from "../menu/search_filter_bar";
import MoreActionWindow from "../windows/more_action";
import { useSearchParams } from "next/navigation";
import RoleContainer from "../object_container/role_container";
import ModifyUserRole from "../windows/modify_user_roles";

function RolesList({ roles, rolesStart, rolesEnd, rolesToChoose }) {
  // Modify role
  const [showModifyRole, setShowModifyRole] = useState(false);
  const [userToModify, setUserToModify] = useState({});
  // More action
  const [showMoreAction, setShowMoreAction] = useState(false);
  // Seleted
  const [selectedQty, setSelectedQty] = useState(0);
  const [selectedKeys] = useState([]);
  // Selected bar button
  const [isClicked, setIsClicked] = useState(false);
  // Nav
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
      {Object.keys(roles).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">Users not found :/</p>
        </Container>
      ) : (
        Object.values(roles)
          .slice(rolesStart, rolesEnd)
          .map((value) => {
            return (
              <RoleContainer
                key={value.userId}
                role={value}
                selected={selectedKeys.indexOf(value.userId) !== -1}
                selectAction={() => {
                  selectedKeys.push(value.userId);
                  setSelectedQty(selectedQty + 1);
                }}
                unselectAction={() => {
                  let index = selectedKeys.indexOf(value.userId);
                  selectedKeys.splice(index, 1);
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
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          let start = page * pagation - pagation;
          let end = page * pagation;
          Object.values(roles)
            .slice(start, end)
            .forEach((e) => selectedKeys.push(e.userId));
          setSelectedQty(selectedKeys.length);
          setShowMoreAction(false);
        }}
        selectAll={() => {
          selectedKeys.splice(0, selectedKeys.length);
          setSelectedQty(0);
          Object.values(roles).forEach((e) => selectedKeys.push(e.userId));
          setSelectedQty(selectedKeys.length);
          setShowMoreAction(false);
        }}
        deselectAll={() => {
          selectedKeys.splice(0, selectedKeys.length);
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
    </Container>
  );
}

RolesList.PropTypes = {
  products: PropTypes.object.isRequired,
  orgView: PropTypes.bool.isRequired,
  productStart: PropTypes.number.isRequired,
  productEnd: PropTypes.number.isRequired,
  rolesToChoose: PropTypes.object.isRequired,
};

export default RolesList;
