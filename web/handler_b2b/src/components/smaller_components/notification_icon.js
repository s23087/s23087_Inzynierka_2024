import PropTypes from "prop-types";

/**
 * Small circle with number or notification. When over 9 it will show as 9+.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.qty Unread notification quantity.
 * @param {string} [props.top_value="-3px"] Distance from top.
 * @param {string} [props.right_value="-7px"] Distance from right.
 * @return {JSX.Element} span element
 */
function NotificationBadge({ qty, top_value = "-3px", right_value = "-7px" }) {
  const spanStyle = {
    height: "22px",
    width: "22px",
    position: "absolute",
    top: top_value,
    right: right_value,
  };

  if (qty <= 0) {
    return;
  }

  return (
    <span
      className="sec-blue-bg d-flex align-items-center justify-content-center rounded-circle"
      style={spanStyle}
    >
      <p className="mb-0 main-text small-text">{qty > 9 ? "9+" : qty}</p>
    </span>
  );
}

NotificationBadge.propTypes = {
  qty: PropTypes.number.isRequired,
  top_value: PropTypes.string,
  right_value: PropTypes.string,
};

export default NotificationBadge;
