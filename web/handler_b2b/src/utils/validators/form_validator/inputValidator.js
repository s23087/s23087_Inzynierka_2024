import validators from "../validator";

/**
 * Checks if string is not empty and do not exceed given length. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 * @param  {Number} maxLength Max length allowed for given string
 */
function normalStringValidator(string, setter, maxLength) {
  if (
    validators.lengthSmallerThen(string, maxLength) &&
    validators.stringIsNotEmpty(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string is not empty, do not exceed given length and do not contain numbers. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 * @param  {Number} maxLength Max length allowed for given string
 */
function noNumberStringValidator(string, setter, maxLength) {
  if (
    validators.lengthSmallerThen(string, maxLength) &&
    validators.stringIsNotEmpty(string) &&
    validators.haveNoNumbers(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string is not empty, do not exceed given length and contain only numbers. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 * @param  {Number} maxLength Max length allowed for given string
 */
function onlyNumberStringValidator(string, setter, maxLength) {
  if (
    validators.lengthSmallerThen(string, maxLength) &&
    validators.stringIsNotEmpty(string) &&
    validators.haveOnlyNumbers(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string is a number and is not empty. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 */
function onlyNumberValidator(string, setter) {
  if (
    validators.stringIsNotEmpty(string) &&
    validators.haveOnlyNumbers(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string is a number and do not exceed given length. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 * @param  {Number} maxLength Max length allowed for given string
 */
function emptyNumberStringValidator(string, setter, maxLength) {
  if (
    validators.lengthSmallerThen(string, maxLength) &&
    validators.haveOnlyNumbers(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string match email pattern and do not exceed given length. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 * @param  {Number} maxLength Max length allowed for given string
 */
function emailValidator(string, setter, maxLength) {
  if (
    validators.lengthSmallerThen(string, maxLength) &&
    validators.isEmail(string) &&
    validators.stringIsNotEmpty(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string match decimal number pattern (with dot as separator and two decimal places) and is not empty. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 */
function decimalValidator(string, setter) {
  if (validators.isPriceFormat(string) && validators.stringIsNotEmpty(string)) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string do not have special characters that disrupts path string. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 */
function noPathCharactersValidator(string, setter) {
  if (
    validators.noPathCharacters(string) &&
    validators.stringIsNotEmpty(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string is not empty. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 */
function isEmptyValidator(string, setter) {
  if (validators.stringIsNotEmpty(string)) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Object that contains function to validate input strings with help of useState function.
 */
const InputValidator = {
  normalStringValidator: normalStringValidator,
  onlyNumberStringValidator: onlyNumberStringValidator,
  emptyNumberStringValidator: emptyNumberStringValidator,
  emailValidator,
  decimalValidator,
  onlyNumberValidator: onlyNumberValidator,
  noPathCharactersValidator,
  noNumberStringValidator: noNumberStringValidator,
  isEmptyValidator,
};

export default InputValidator;
