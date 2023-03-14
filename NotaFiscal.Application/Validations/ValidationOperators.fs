module NotaFiscal.WebApplication.Validations.ValidationOperators

open NotaFiscal.WebApplication.Validations.ValidationUtils

let bind
    (binder: Input<'value> -> ValidationResult<'valueInput, 'valueResult>)
    (outputPreviousValidation: ValidationResult<'value, 'valueInput>)
    =
    match outputPreviousValidation with
    | Ok(fieldName, value) -> binder (fieldName, value)
    | Error errorResult ->
        let (fieldName, value), errors = errorResult
        let binderResult = binder (fieldName, value)

        match binderResult with
        | Ok _ -> binderResult
        | Error(_, newErrors) -> Error((fieldName, value), errors @ newErrors)

let (>>=) input binder = bind binder input
