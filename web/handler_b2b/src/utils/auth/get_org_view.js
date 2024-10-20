export default function getOrgView(role, orgActivated) {
  return (
    (role === "Admin" && orgActivated) ||
    role === "Accountant" ||
    role === "Warehouse Manager"
  );
}
