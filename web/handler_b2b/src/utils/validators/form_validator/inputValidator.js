import validators from "../validator";

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

function decimalValidator(string, setter) {
  if (validators.isPriceFormat(string) && validators.stringIsNotEmpty(string)) {
    setter(false);
  } else {
    setter(true);
  }
}

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

function isEmptyValidator(string, setter) {
  if (
    validators.stringIsNotEmpty(string)
  ) {
    setter(false);
  } else {
    setter(true);
  }
}

const InputValidtor = {
  normalStringValidtor,
  onlyNumberStringValidtor,
  emptyNumberStringValidtor,
  emailValidator,
  decimalValidator,
  onlyNumberValidtor,
  noPathCharactersValidator,
  noNumberStringValidtor,
  isEmptyValidator
};

export default InputValidtor;
