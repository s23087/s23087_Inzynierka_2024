"use client";

/**
 * Calculate starting and ending points for array of object depending on chosen page and pagination.
 * If user is on first page and pagination is equal 10, then starting point in array will be 0 and ending will be 9.
 * @return {{start: Number, end: Number}} Return start and end of array
 */
export default function getPaginationInfo(params) {
  const accessibleParams = new URLSearchParams(params);
  let pagination = accessibleParams.get("pagination")
    ? accessibleParams.get("pagination")
    : 10;
  let page = accessibleParams.get("page") ? accessibleParams.get("page") : 1;
  return {
    start: page * pagination - pagination,
    end: page * pagination - 1,
  };
}
