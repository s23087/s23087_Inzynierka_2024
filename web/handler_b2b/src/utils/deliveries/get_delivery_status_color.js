/**
 * Depend on chosen status will return object with styling properties for element. If status is not found will by default return red background and white text.
 * @return {{backgroundColor: string, color: string}} Object that contain style properties for chosen status.
 */
export default function getDeliveryStatusColor(statusName) {
  if (statusName === "In transport" || statusName === "Preparing") {
    return {
      backgroundColor: "var(--main-yellow)",
      color: "var(--text-black-color)",
    };
  }
  if (statusName === "Fulfilled") {
    return {
      backgroundColor: "var(--main-green)",
      color: "var(--text-black-color)",
    };
  }
  // Delivered with issues && Rejected
  return {
    backgroundColor: "var(--sec-red)",
    color: "var(--text-main-color)",
  };
}
