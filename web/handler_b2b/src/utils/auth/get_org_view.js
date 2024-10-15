export default function getOrgView(role, orgActivated) {
  if (role === "Admin" && orgActivated) {
    return true;
  }
  if (role === "Accountant" || role === "Warehouse Manager") {
    return true;
  }
  return false;
}
