export default function getOrgView(role, orgActivated) {
  if (role === "Admin" && orgActivated) {
    return true;
  }
  if (role === "Accountant") {
    return true;
  }
  return false;
}
