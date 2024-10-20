export default function getDeliveryStatusColor(statusName) {
  switch (statusName) {
    case "In transport":
      return {
        backgroundColor: "var(--main-yellow)",
        color: "var(--text-black-color)",
      };
    case "Preparing":
      return {
        backgroundColor: "var(--main-yellow)",
        color: "var(--text-black-color)",
      };
    case "Fulfilled":
      return {
        backgroundColor: "var(--main-green)",
        color: "var(--text-black-color)",
      };
    default:
      // Delivered with issues && Rejected
      return {
        backgroundColor: "var(--sec-red)",
        color: "var(--text-main-color)",
      };
  }
}
