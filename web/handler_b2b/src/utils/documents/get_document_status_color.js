export default function getDocumentStatusStyle(statusName) {
  switch (statusName) {
    case "Paid":
      return {
        backgroundColor: "var(--main-green)",
      };
    case "Unpaid":
      return {
        backgroundColor: "var(--main-yellow)",
      };
    case "Due to":
      return {
        backgroundColor: "var(--sec-red)",
        color: "var(--text-main-color)",
      };
    case "In system":
      return {
        backgroundColor: "var(--main-green)",
        justifyContent: "center",
      };
    case "Requested":
      return {
        backgroundColor: "var(--main-yellow)",
        justifyContent: "center",
      };
    case "Not in system":
      return {
        backgroundColor: "var(--sec-red)",
        color: "var(--text-main-color)",
        justifyContent: "center",
      };
    default:
      return {
        backgroundColor: "white",
      };
  }
}
