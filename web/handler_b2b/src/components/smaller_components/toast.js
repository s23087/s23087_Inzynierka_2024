import { Toast } from "react-bootstrap";
import PropTypes from "prop-types";

const toastStyle = {
  maxWidth: "180px",
  marginBottom: "90px",
  whiteSpace: "pre-line",
};

function ErrorToast({ showToast, message, onHideFun }) {
  return (
    <Toast
      show={showToast}
      className="position-absolute bottom-0 end-0 me-3 w-auto main-red-bg border"
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

ErrorToast.PropTypes = {
  showToast: PropTypes.bool.isRequired,
  message: PropTypes.string.isRequired,
  onHideFun: PropTypes.func.isRequired,
};

function SuccessToast({ showToast, message, onHideFun }) {
  return (
    <Toast
      show={showToast}
      className="position-absolute bottom-0 end-0 me-3 w-auto main-green-bg border"
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

SuccessToast.PropTypes = {
  showToast: PropTypes.bool.isRequired,
  message: PropTypes.string.isRequired,
  onHideFun: PropTypes.func.isRequired,
};

const Toastes = {
  ErrorToast,
  SuccessToast,
};

export default Toastes;
