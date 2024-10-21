import validators from "@/utils/validators/validator";

function setDateFilter(newParams) {
  let dateL = document.getElementById("dateL").value;
  if (dateL) newParams.set("dateL", dateL);
  if (!dateL) newParams.delete("dateL");
  let dateG = document.getElementById("dateG").value;
  if (dateG) newParams.set("dateG", dateG);
  if (!dateG) newParams.delete("dateG");
}

function setQtyFilter(newParams) {
  let qtyG = document.getElementById("qtyG").value;
  if (qtyG) newParams.set("qtyG", qtyG);
  if (!qtyG) newParams.delete("qtyG");
  let qtyL = document.getElementById("qtyL").value;
  if (qtyL) newParams.set("qtyL", qtyL);
  if (!qtyL) newParams.delete("qtyL");
}

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

function setRecipientFilter(newParams) {
  let recipient = document.getElementById("recipient").value;
  if (recipient !== "none") newParams.set("recipient", recipient);
  if (recipient === "none") newParams.delete("recipient");
}

function setCurrencyFilter(newParams) {
  let currencyFilter = document.getElementById("currencyFilter").value;
  if (currencyFilter !== "none") newParams.set("currency", currencyFilter);
  if (currencyFilter === "none") newParams.delete("currency");
}

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

function setEanFilter(newParams) {
  let eanFilter = document.getElementById("eanFilter").value;
  if (validators.haveOnlyNumbers(eanFilter) && eanFilter)
    newParams.set("ean", eanFilter);
  if (!eanFilter) newParams.delete("ean");
}

function setStatusFilter(newParams) {
  let statusFilter = document.getElementById("filterStatus").value;
  if (statusFilter !== "none") newParams.set("status", statusFilter);
  if (statusFilter === "none") newParams.delete("status");
}

function setTypeFilter(newParams) {
  let typeFilter = document.getElementById("typeFilter").value;
  if (typeFilter !== "none") newParams.set("type", typeFilter);
  if (typeFilter === "none") newParams.delete("type");
}

function setModifiedFilter(newParams) {
  let modifiedL = document.getElementById("modifiedL").value;
  if (modifiedL) newParams.set("modifiedL", modifiedL);
  if (!modifiedL) newParams.delete("modifiedL");
  let modifiedG = document.getElementById("modifiedG").value;
  if (modifiedG) newParams.set("modifiedG", modifiedG);
  if (!modifiedG) newParams.delete("modifiedG");
}

function setCreatedFilter(newParams) {
  let createdG = document.getElementById("createdG").value;
  if (createdG) newParams.set("createdG", createdG);
  if (!createdG) newParams.delete("createdG");
  let createdL = document.getElementById("createdL").value;
  if (createdL) newParams.set("createdL", createdL);
  if (!createdL) newParams.delete("createdL");
}

function setSortFilter(newParams, isAsc) {
  let sort = document.getElementById("sortValue").value;
  if (sort != "None") {
    sort = isAsc ? "A" + sort : "D" + sort;
    newParams.set("orderBy", sort);
  } else {
    newParams.delete("orderBy");
  }
}

function setSourceFilter(newParams) {
  let source = document.getElementById("source").value;
  if (source !== "none") newParams.set("source", source);
  if (source === "none") newParams.delete("source");
}

function setRequestStatusFilter(newParams) {
  let requestStatus = document.getElementById("requestStatus").value;
  if (requestStatus !== "none") newParams.set("requestStatus", requestStatus);
  if (requestStatus === "none") newParams.delete("requestStatus");
}

function setDueFilter(newParams) {
  let dueL = document.getElementById("dueL").value;
  if (dueL) newParams.set("dueL", dueL);
  if (!dueL) newParams.delete("dueL");
  let dueG = document.getElementById("dueG").value;
  if (dueG) newParams.set("dueG", dueG);
  if (!dueG) newParams.delete("dueG");
}

function setPaymentStatusFilter(newParams) {
  let paymentStatus = document.getElementById("paymentStatus").value;
  if (paymentStatus !== "none") newParams.set("paymentStatus", paymentStatus);
  if (paymentStatus === "none") newParams.delete("paymentStatus");
}

function setCompanyFilter(newParams) {
  let company = document.getElementById("company").value;
  if (company !== "none") newParams.set("company", company);
  if (company === "none") newParams.delete("company");
}

function setWaybillFilter(newParams) {
  let waybill = document.getElementById("waybill").value;
  if (waybill) newParams.set("waybill", waybill);
  if (!waybill) newParams.delete("waybill");
}

function setDeliveredGreaterFilter(newParams) {
  let deliveredG = document.getElementById("deliveredG").value;
  if (deliveredG) newParams.set("deliveredG", deliveredG);
  if (!deliveredG) newParams.delete("deliveredG");
}

function setDeliveredLowerFilter(newParams) {
  let deliveredL = document.getElementById("deliveredL").value;
  if (deliveredL) newParams.set("deliveredL", deliveredL);
  if (!deliveredL) newParams.delete("deliveredL");
}

function setEstimatedGreaterFilter(newParams) {
  let estimatedG = document.getElementById("estimatedG").value;
  if (estimatedG) newParams.set("estimatedG", estimatedG);
  if (!estimatedG) newParams.delete("estimatedG");
}

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
