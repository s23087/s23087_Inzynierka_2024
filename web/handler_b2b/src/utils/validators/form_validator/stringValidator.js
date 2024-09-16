import validators from "../validator";

function normalStringValidtor(string, setter, maxLenght){
    if (
        validators.lengthSmallerThen(string, maxLenght) &&
        validators.stringIsNotEmpty(string)
      ) {
        setter(false);
      } else {
        setter(true);
      }
}

function onlyNumberStringValidtor(string, setter, maxLenght){
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

function emptyNumberStringValidtor(string, setter, maxLenght){
  if (
      validators.lengthSmallerThen(string, maxLenght) &&
      validators.haveOnlyNumbers(string)
    ) {
      setter(false);
    } else {
      setter(true);
    }
}

function emailValidator(string, setter, maxLenght){
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

const StringValidtor = {
  normalStringValidtor,
  onlyNumberStringValidtor,
  emptyNumberStringValidtor,
  emailValidator
}

export default StringValidtor