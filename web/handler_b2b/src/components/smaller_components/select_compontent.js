"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import {
  Container,
  Row,
  Col,
  DropdownButton,
  Dropdown,
  Button,
} from "react-bootstrap";

function SelectComponent({ selectedQty }) {
  const [isClicked, setIsClicked] = useState(false);
  const containerStyle = {
    height: "67px",
    display: selectedQty > 0 ? "block" : "none",
  };
  const buttonStyle = {
    width: "154px",
    height: "48px",
  };
  return (
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
  );
}

SelectComponent.PropTypes = {
  selectedQty: PropTypes.number.isRequired,
};

export default SelectComponent;
