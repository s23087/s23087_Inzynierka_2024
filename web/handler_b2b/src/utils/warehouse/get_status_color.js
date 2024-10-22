/**
 * Return name of css variable. If name do not match correct name will return always green (default is the colour of "Fulfilled" status). Status names: In delivery, On request, Unavailable.
 * @param  {string} statusName The name of status.
 * Completed will always be true, to deliver information to component that action has been completed.
 */
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
