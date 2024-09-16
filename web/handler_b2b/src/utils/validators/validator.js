function isEmail(str) {
  const reg = new RegExp(
    /^(([^<>()\]\\.,;:\s@"]+(\.[^<>()\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
  );
  return reg.test(str);
}

function isPriceFormat(str) {
  const reg = /^\d+(\.\d{1,2})?$/;
  return reg.test(str);
}

function haveNoNumbers(str) {
  const reg = /^[a-zA-Z]+$/;
  return reg.test(str);
}

function haveOnlyNumbers(str) {
  const reg = /^\d*$/;
  return reg.test(str);
}

function lengthSmallerThen(str, length) {
  return str.length <= length;
}

function stringAreEqual(str1, str2) {
  return str1 === str2;
}

function isEmpty(id) {
  return document.getElementById(id).value.length <= 0;
}

function stringIsNotEmpty(str) {
  return str.length > 0;
}

function validate(str, func, maxLength) {
  return func(str) && lengthSmallerThen(str, maxLength);
}

const validators = {
  isEmail,
  haveNoNumbers,
  haveOnlyNumbers,
  lengthSmallerThen,
  stringAreEqual,
  isEmpty,
  stringIsNotEmpty,
  validate,
  isPriceFormat,
};

export default validators;
