export default function getDeliveryStatusColor(statusName) {
  switch (statusName) {
    case "In transport":
      return {
        backgroundColor: "var(--main-yellow)",
        color: "var(--text-black-color)",
      };
    case "Delivered with issues":
      return {
        backgroundColor: "var(--sec-red)",
        color: "var(--text-main-color)",
      };
    case "Preparing":
      return {
        backgroundColor: "var(--main-yellow)",
        color: "var(--text-black-color)",
      };
    case "Rejected":
      return {
        backgroundColor: "var(--sec-red)",
        color: "var(--text-main-color)",
      };
    default:
      return {
        backgroundColor: "var(--main-green)",
        color: "var(--text-black-color)",
      };
  }
}
