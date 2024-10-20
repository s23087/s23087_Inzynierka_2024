export default function getOrgView(role, orgActivated) {
  if (
    (role === "Admin" && orgActivated) ||
    role === "Accountant" ||
    role === "Warehouse Manager"
  ) {
    return true;
  }
  return false;
}
