import PropTypes from "prop-types";

function NotificationBadge({ qty }) {
  const spanStyle = {
    height: "22px",
    width: "22px",
    position: "absolute",
    top: "-3px",
    right: "-7px",
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

NotificationBadge.PropTypes = {
  qty: PropTypes.number.isRequired,
};

export default NotificationBadge;
