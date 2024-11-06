import validators from "@/utils/validators/validator";

/**
 * Get the values from elements using "dateL" and "dateG" ids, then set them as query parameters if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setDateFilter(newParams) {
  let dateL = document.getElementById("dateL").value;
  if (dateL) newParams.set("dateL", dateL);
  if (!dateL) newParams.delete("dateL");
  let dateG = document.getElementById("dateG").value;
  if (dateG) newParams.set("dateG", dateG);
  if (!dateG) newParams.delete("dateG");
}

/**
 * Get the values from elements using "qtyG" and "qtyL" ids, then set them as query parameters if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setQtyFilter(newParams) {
  let qtyG = document.getElementById("qtyG").value;
  if (qtyG) newParams.set("qtyG", qtyG);
  if (!qtyG) newParams.delete("qtyG");
  let qtyL = document.getElementById("qtyL").value;
  if (qtyL) newParams.set("qtyL", qtyL);
  if (!qtyL) newParams.delete("qtyL");
}

/**
 * Get the values from elements using "totalL" and "totalG" ids, then set them as query parameters if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setTotalFilter(newParams) {
  let totalL = document.getElementById("totalL").value;
  if (validators.haveOnlyNumbers(totalL) && totalL)
    newParams.set("totalL", totalL);
  if (!totalL) newParams.delete("totalL");
  let totalG = document.getElementById("totalG").value;
  if (validators.haveOnlyNumbers(totalG) && totalG)
    newParams.set("totalG", totalG);
  if (!totalG) newParams.delete("totalG");
}

/**
 * Get the values from elements using "recipient" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setRecipientFilter(newParams) {
  let recipient = document.getElementById("recipient").value;
  if (recipient !== "none") newParams.set("recipient", recipient);
  if (recipient === "none") newParams.delete("recipient");
}

/**
 * Get the values from elements using "currencyFilter" id, then set them as query parameter "currency" if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setCurrencyFilter(newParams) {
  let currencyFilter = document.getElementById("currencyFilter").value;
  if (currencyFilter !== "none") newParams.set("currency", currencyFilter);
  if (currencyFilter === "none") newParams.delete("currency");
}

/**
 * Get the values from elements using "priceL" and "priceG" ids, then set them as query parameters if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setPriceFilter(newParams) {
  let priceLess = document.getElementById("priceL").value;
  if (validators.haveOnlyNumbers(priceLess) && priceLess)
    newParams.set("priceL", priceLess);
  if (!priceLess) newParams.delete("priceL");
  let priceGreater = document.getElementById("priceG").value;
  if (validators.haveOnlyNumbers(priceGreater) && priceGreater)
    newParams.set("priceG", priceGreater);
  if (!priceGreater) newParams.delete("priceG");
}

/**
 * Get the values from elements using "eanFilter" id, then set them as query parameter "ean" if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setEanFilter(newParams) {
  let eanFilter = document.getElementById("eanFilter").value;
  if (validators.haveOnlyNumbers(eanFilter) && eanFilter)
    newParams.set("ean", eanFilter);
  if (!eanFilter) newParams.delete("ean");
}

/**
 * Get the values from elements using "filterStatus" id, then set them as query parameter "status" if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setStatusFilter(newParams) {
  let statusFilter = document.getElementById("filterStatus").value;
  if (statusFilter !== "none") newParams.set("status", statusFilter);
  if (statusFilter === "none") newParams.delete("status");
}

/**
 * Get the values from elements using "typeFilter" id, then set them as query parameter "type" if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setTypeFilter(newParams) {
  let typeFilter = document.getElementById("typeFilter").value;
  if (typeFilter !== "none") newParams.set("type", typeFilter);
  if (typeFilter === "none") newParams.delete("type");
}

/**
 * Get the values from elements using "modifiedL" and "modifiedG" ids, then set them as query parameters if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setModifiedFilter(newParams) {
  let modifiedL = document.getElementById("modifiedL").value;
  if (modifiedL) newParams.set("modifiedL", modifiedL);
  if (!modifiedL) newParams.delete("modifiedL");
  let modifiedG = document.getElementById("modifiedG").value;
  if (modifiedG) newParams.set("modifiedG", modifiedG);
  if (!modifiedG) newParams.delete("modifiedG");
}

/**
 * Get the values from elements using "createdG" and "createdL" ids, then set them as query parameters if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setCreatedFilter(newParams) {
  let createdG = document.getElementById("createdG").value;
  if (createdG) newParams.set("createdG", createdG);
  if (!createdG) newParams.delete("createdG");
  let createdL = document.getElementById("createdL").value;
  if (createdL) newParams.set("createdL", createdL);
  if (!createdL) newParams.delete("createdL");
}

/**
 * Get the values from elements using "sortValue" id, then set them as query parameter "orderBy" if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setSortFilter(newParams, isAsc) {
  let sort = document.getElementById("sortValue").value;
  if (sort != "None") {
    sort = isAsc ? "A" + sort : "D" + sort;
    newParams.set("orderBy", sort);
  } else {
    newParams.delete("orderBy");
  }
}

/**
 * Get the values from elements using "source" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setSourceFilter(newParams) {
  let source = document.getElementById("source").value;
  if (source !== "none") newParams.set("source", source);
  if (source === "none") newParams.delete("source");
}

/**
 * Get the values from elements using "requestStatus" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setRequestStatusFilter(newParams) {
  let requestStatus = document.getElementById("requestStatus").value;
  if (requestStatus !== "none") newParams.set("requestStatus", requestStatus);
  if (requestStatus === "none") newParams.delete("requestStatus");
}

/**
 * Get the values from elements using "dueL" and "dueG" ids, then set them as query parameters if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setDueFilter(newParams) {
  let dueL = document.getElementById("dueL").value;
  if (dueL) newParams.set("dueL", dueL);
  if (!dueL) newParams.delete("dueL");
  let dueG = document.getElementById("dueG").value;
  if (dueG) newParams.set("dueG", dueG);
  if (!dueG) newParams.delete("dueG");
}

/**
 * Get the values from elements using "paymentStatus" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setPaymentStatusFilter(newParams) {
  let paymentStatus = document.getElementById("paymentStatus").value;
  if (paymentStatus !== "none") newParams.set("paymentStatus", paymentStatus);
  if (paymentStatus === "none") newParams.delete("paymentStatus");
}

/**
 * Get the values from elements using "company" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setCompanyFilter(newParams) {
  let company = document.getElementById("company").value;
  if (company !== "none") newParams.set("company", company);
  if (company === "none") newParams.delete("company");
}

/**
 * Get the values from elements using "waybill" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setWaybillFilter(newParams) {
  let waybill = document.getElementById("waybill").value;
  if (waybill) newParams.set("waybill", waybill);
  if (!waybill) newParams.delete("waybill");
}

/**
 * Get the values from elements using "deliveredG" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setDeliveredGreaterFilter(newParams) {
  let deliveredG = document.getElementById("deliveredG").value;
  if (deliveredG) newParams.set("deliveredG", deliveredG);
  if (!deliveredG) newParams.delete("deliveredG");
}

/**
 * Get the values from elements using "deliveredL" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setDeliveredLowerFilter(newParams) {
  let deliveredL = document.getElementById("deliveredL").value;
  if (deliveredL) newParams.set("deliveredL", deliveredL);
  if (!deliveredL) newParams.delete("deliveredL");
}

/**
 * Get the values from elements using "estimatedG" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setEstimatedGreaterFilter(newParams) {
  let estimatedG = document.getElementById("estimatedG").value;
  if (estimatedG) newParams.set("estimatedG", estimatedG);
  if (!estimatedG) newParams.delete("estimatedG");
}

/**
 * Get the values from elements using "estimatedL" id, then set them as query parameter if they exist and have correct value.
 * @param {URLSearchParams} newParams Object to pass new search parameters
 */
function setEstimatedLowerFilter(newParams) {
  let estimatedL = document.getElementById("estimatedL").value;
  if (estimatedL) newParams.set("estimatedL", estimatedL);
  if (!estimatedL) newParams.delete("estimatedL");
}

const SetQueryFunc = {
  setDateFilter,
  setQtyFilter,
  setTotalFilter,
  setRecipientFilter,
  setCurrencyFilter,
  setPriceFilter,
  setEanFilter,
  setStatusFilter,
  setTypeFilter,
  setModifiedFilter,
  setCreatedFilter,
  setSortFilter,
  setSourceFilter,
  setRequestStatusFilter,
  setDueFilter,
  setPaymentStatusFilter,
  setCompanyFilter,
  setWaybillFilter,
  setDeliveredGreaterFilter,
  setDeliveredLowerFilter,
  setEstimatedGreaterFilter,
  setEstimatedLowerFilter,
};

export default SetQueryFunc;
