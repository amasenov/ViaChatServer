/* * * * * * * * * * * * *
 * DEVELOPMENT UTILITIES *
 * * * * * * * * * * * * */
export const isUndefined = value => (typeof value === 'undefined');
export const isDefined = value => !isUndefined(value);
export const isNull = value => (!value && typeof value === 'object');
export const isNumber = value => (typeof value === 'number');
// No support for Number.isInteger by IE
// export const isInteger = (value) => (Number.isInteger(value));
export const isInteger = value => (isNumber(value) && isFinite(value) && Math.floor(value) === value);
export const isBoolean = value => (typeof value === 'boolean');
export const isString = value => (typeof value === 'string');
export const isFullString = value => (isString(value) && !!value.trim().length);
export const isEmptyString = value => (isString(value) && !value.trim().length);
export const isArray = value => (typeof value === 'object' && Array.isArray(value));
export const isFullArray = value => (isArray(value) && !!value.length);
export const isEmptyArray = value => (isArray(value) && !value.length);
export const isSymbol = value => (typeof value === 'symbol');
export const isObject = value => (typeof value === 'object' && !isNull(value) && !isArray(value));
export const isEmptyObject = value => (isObject(value) && Object.keys(value).length === 0);
export const isFunction = value => (typeof value === 'function');
export const areEqual = (first, second) => JSON.stringify(first) === JSON.stringify(second);