"use client";

import PropTypes from "prop-types";

function ErrorMessage({ message, messageStatus }) {
  const hidden = {
    display: "none",
  };
  const unhidden = {
    display: "block",
  };
  return (
    <p
      className="text-start mb-1 red-sec-text small-text"
      style={messageStatus ? unhidden : hidden}
    >
      {message}
    </p>
  );
}

ErrorMessage.propTypes = {
  message: PropTypes.string.isRequired,
  messageStatus: PropTypes.bool.isRequired,
};

export default ErrorMessage;
