/**
 * Checks whenever user can get organization view on site.
 * @param {string} role User role.
 * @param {boolean} orgActivated If user activated org view true, otherwise false.
 * @return {Promise<boolean>} True if user can use orgView, false if cannot.
 */
export default function getOrgView(role, orgActivated) {
  return (
    (role === "Admin" && orgActivated) ||
    role === "Accountant" ||
    role === "Warehouse Manager"
  );
}
