/**
 * Test string for email pattern.
 * @param  {string} str 
 * @return {boolean}      Result of function regex function test.
 */
function isEmail(str) {
  const reg = new RegExp(
    /^(([^<>()\]\\.,;:\s@"]+(\.[^<>()\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
  );
  return reg.test(str);
}

/**
 * Test string for decimal number with two decimal places pattern.
 * @param  {string} str 
 * @return {boolean}      Result of function regex function test.
 */
function isPriceFormat(str) {
  const reg = /^\d+(\.\d{1,2})?$/;
  return reg.test(str);
}

/**
 * Test if string do not contain special characters.
 * @param  {string} str 
 * @return {boolean}      Result of function regex function test.
 */
function noPathCharacters(str) {
  const reg = /^[0-9a-zA-Z]*$/;
  return reg.test(str);
}

/**
 * Test if string contains only letters a-z and A-Z.
 * @param  {string} str 
 * @return {boolean}      Result of function regex function test.
 */
function haveNoNumbers(str) {
  const reg = /^[a-zA-Z]+$/;
  return reg.test(str);
}

/**
 * Test if string is a postivie integer.
 * @param  {string} str 
 * @return {boolean}      Result of function regex function test.
 */
function haveOnlyPositiveNumbers(str) {
  const reg = /^\d*$/;
  return reg.test(str);
}

/**
 * Test if string is a integer.
 * @param  {string} str 
 * @return {boolean}      Result of function regex function test.
 */
function haveOnlyIntegers(str) {
  const reg = /^-?d*$/;
  return reg.test(str);
}

/**
 * Test if string do not exceed given length.
 * @param  {string} str 
 * @return {boolean}
 */
function lengthSmallerThen(str, length) {
  return str.length <= length;
}

/**
 * Test if strings are equal.
 * @param  {string} str1 
 * @param  {string} str2 
 * @return {boolean}
 */
function stringAreEqual(str1, str2) {
  return str1 === str2;
}

/**
 * Test if string is not empty.
 * @param  {string} id Html element id 
 * @return {boolean}
 */
function isEmpty(id) {
  return document.getElementById(id).value.length <= 0;
}

/**
 * Test if string is not empty.
 * @param  {string} str 
 * @return {boolean}
 */
function stringIsNotEmpty(str) {
  return str.length > 0;
}

const validators = {
  isEmail,
  haveNoNumbers,
  haveOnlyNumbers: haveOnlyPositiveNumbers,
  lengthSmallerThen,
  stringAreEqual,
  isEmpty,
  stringIsNotEmpty,
  isPriceFormat,
  haveOnlyIntegers,
  noPathCharacters,
};

export default validators;
