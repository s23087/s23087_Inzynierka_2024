import { Toast } from "react-bootstrap";
import PropTypes from "prop-types";

const toastStyle = {
  maxWidth: "180px",
  marginBottom: "90px",
  whiteSpace: "pre-line",
};

/**
 * Create Toast element with error message.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showToast True if toast should be visible, otherwise false.
 * @param {string} props.message Error message.
 * @param {Function} props.onHideFun Function that will activate after clicking "X" button.
 * @return {JSX.Element} Toast element
 */
function ErrorToast({ showToast, message, onHideFun }) {
  return (
    <Toast
      show={showToast}
      className="position-absolute bottom-0 end-0 me-3 w-auto main-red-bg border z-2"
      onClose={onHideFun}
      style={toastStyle}
    >
      <Toast.Header className="main-red-bg">
        <p className="mb-0 main-text me-auto">Error</p>
      </Toast.Header>
      <Toast.Body className="main-text">{message}</Toast.Body>
    </Toast>
  );
}

ErrorToast.propTypes = {
  showToast: PropTypes.bool.isRequired,
  message: PropTypes.string.isRequired,
  onHideFun: PropTypes.func.isRequired,
};

/**
 * Create Toast element with success message.
 * @component
 * @param {object} props Component props
 * @param {boolean} props.showToast True if toast should be visible, otherwise false.
 * @param {string} props.message Success message.
 * @param {Function} props.onHideFun Function that will activate after clicking "X" button.
 * @return {JSX.Element} Toast element
 */
function SuccessToast({ showToast, message, onHideFun }) {
  return (
    <Toast
      show={showToast}
      className="position-absolute bottom-0 end-0 me-3 w-auto main-green-bg border z-2"
      onClose={onHideFun}
      style={toastStyle}
    >
      <Toast.Header className="main-green-bg">
        <p className="mb-0 main-text me-auto">Success!</p>
      </Toast.Header>
      <Toast.Body className="main-text">{message}</Toast.Body>
    </Toast>
  );
}

SuccessToast.propTypes = {
  showToast: PropTypes.bool.isRequired,
  message: PropTypes.string.isRequired,
  onHideFun: PropTypes.func.isRequired,
};

const Toastes = {
  ErrorToast,
  SuccessToast,
};

export default Toastes;
