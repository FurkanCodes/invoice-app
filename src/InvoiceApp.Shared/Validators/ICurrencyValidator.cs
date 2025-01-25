using InvoiceApp.Shared.Validators;

public interface ICurrencyValidator
{
    bool IsValidCode(string code);
    bool IsValidSymbol(string symbol);
}


public class CurrencyValidatorService : ICurrencyValidator
{
    public bool IsValidCode(string code) =>
        CurrencyValidator.IsValidCurrencyCode(code);

    public bool IsValidSymbol(string symbol) =>
        CurrencyValidator.IsValidCurrencySymbol(symbol);
}
