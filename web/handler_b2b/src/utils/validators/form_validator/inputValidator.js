import validators from "../validator";

/**
 * Checks if string is not empty and do not exceed given length. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 * @param  {Number} maxLenght Max length allowed for given string
 */
function normalStringValidtor(string, setter, maxLenght) {
  if (
    validators.lengthSmallerThen(string, maxLenght) &&
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
 * @param  {Number} maxLenght Max length allowed for given string
 */
function noNumberStringValidtor(string, setter, maxLenght) {
  if (
    validators.lengthSmallerThen(string, maxLenght) &&
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
 * @param  {Number} maxLenght Max length allowed for given string
 */
function onlyNumberStringValidtor(string, setter, maxLenght) {
  if (
    validators.lengthSmallerThen(string, maxLenght) &&
    validators.stringIsNotEmpty(string) &&
    validators.haveOnlyNumbers(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string is a number. Then use given setter to return outcome.
 * @param  {string} string
 * @param  {Function} setter UseState function.
 */
function onlyNumberValidtor(string, setter) {
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
 * @param  {Number} maxLenght Max length allowed for given string
 */
function emptyNumberStringValidtor(string, setter, maxLenght) {
  if (
    validators.lengthSmallerThen(string, maxLenght) &&
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
 * @param  {Number} maxLenght Max length allowed for given string
 */
function emailValidator(string, setter, maxLenght) {
  if (
    validators.lengthSmallerThen(string, maxLenght) &&
    validators.isEmail(string) &&
    validators.stringIsNotEmpty(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

/**
 * Checks if string match decimal number pattern (with dot as separator and two decimal places). Then use given setter to return outcome.
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
const InputValidtor = {
  normalStringValidtor,
  onlyNumberStringValidtor,
  emptyNumberStringValidtor,
  emailValidator,
  decimalValidator,
  onlyNumberValidtor,
  noPathCharactersValidator,
  noNumberStringValidtor,
  isEmptyValidator,
};

export default InputValidtor;
