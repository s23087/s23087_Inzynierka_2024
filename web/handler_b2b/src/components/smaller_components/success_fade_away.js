"use client";

import PropTypes from "prop-types";
import { Col } from "react-bootstrap";

/**
 * Return element showing success message that will fade away after a moment.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showSuccess useState boolean. True if message should be shown.
 * @param {Function} props.setShowSuccess useState setter of showSuccess boolean.
 * @return {JSX.Element} Col element
 */
function SuccessFadeAway({ showSuccess, setShowSuccess }) {
  const transition = {
    opacity: 0,
    transition: "all 250ms linear 1.5s",
  };
  const beforeTransition = {
    opacity: 100,
    transition: "all 200ms linear 0s",
  };
  return (
    <Col className="text-end">
      <p
        className="mb-0 mt-3 green-main-text"
        style={showSuccess ? beforeTransition : transition}
        onTransitionEnd={() => setShowSuccess(false)}
      >
        Success!
      </p>
    </Col>
  );
}

SuccessFadeAway.propTypes = {
  showSuccess: PropTypes.bool.isRequired,
  setShowSuccess: PropTypes.func.isRequired,
};

export default SuccessFadeAway;
