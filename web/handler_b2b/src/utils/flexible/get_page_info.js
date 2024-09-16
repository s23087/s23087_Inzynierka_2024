"use client"

export default function getPagationInfo(params){
  const accessibleParams = new URLSearchParams(params);
  let pagation = accessibleParams.get("pagation")
    ? accessibleParams.get("pagation")
    : 10;
  let page = accessibleParams.get("page") ? accessibleParams.get("page") : 1;
  return {
    start: page * pagation - pagation,
    end: page * pagation,
  }
}