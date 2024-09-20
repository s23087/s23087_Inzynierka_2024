export default function getInvoiceStatusColor(statusName) {
  switch (statusName) {
    case "Paid":
      return "var(--main-green)";
    case "Unpaid":
      return "var(--main-yellow)";
    case "Due to":
      return "var(--sec-red)";
    case "In system":
      return "var(--main-green)";
    case "Requested":
      return "var(--main-yellow)";
    case "Not in system":
      return "var(--sec-red)";
    default:
      return "white";
  }
}
