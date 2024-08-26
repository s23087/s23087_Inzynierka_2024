export default function getStatusColor(statusName) {
  switch (statusName) {
    case "In delivery":
      return "var(--main-yellow)";
    case "On request":
      return "var(--sec-red)";
    case "Unavailable":
      return "var(--sec-grey)";
  }
  return "var(--main-green)";
}
