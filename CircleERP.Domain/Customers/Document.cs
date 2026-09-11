using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Customers;

/// <summary>
/// Documento do cliente: CPF para pessoa fisica, CNPJ para pessoa juridica.
/// </summary>
/// <remarks>
/// Guarda apenas digitos, sem pontuacao, para que "123.456.789-09" e
/// "12345678909" sejam o mesmo documento -- do contrario o mesmo cliente
/// entraria duas vezes no cadastro sem ninguem perceber.
///
/// Os digitos verificadores sao conferidos de verdade. Nao e preciosismo: e o
/// que separa "o campo tem 11 caracteres" de "este CPF pode existir".
/// </remarks>
public sealed class Document : ValueObject
{
    private const int CpfLength = 11;
    private const int CnpjLength = 14;

    private Document(string value, CustomerType type)
    {
        Value = value;
        Type = type;
    }

    /// <summary>Somente digitos.</summary>
    public string Value { get; }

    public CustomerType Type { get; }

    /// <summary>
    /// Cria o documento conferindo que ele corresponde ao tipo informado.
    /// </summary>
    public static Document Create(string? value, CustomerType type)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

        if (digits.Length == 0)
            throw new DomainException("O documento e obrigatorio.");

        return type switch
        {
            CustomerType.Individual => CreateCpf(digits),
            CustomerType.Company => CreateCnpj(digits),
            _ => throw new DomainException($"Tipo de cliente desconhecido: {type}."),
        };
    }

    private static Document CreateCpf(string digits)
    {
        if (digits.Length != CpfLength)
            throw new DomainException("Pessoa fisica exige um CPF com 11 digitos.");

        if (!HasValidCpfCheckDigits(digits))
            throw new DomainException("CPF invalido.");

        return new Document(digits, CustomerType.Individual);
    }

    private static Document CreateCnpj(string digits)
    {
        if (digits.Length != CnpjLength)
            throw new DomainException("Pessoa juridica exige um CNPJ com 14 digitos.");

        if (!HasValidCnpjCheckDigits(digits))
            throw new DomainException("CNPJ invalido.");

        return new Document(digits, CustomerType.Company);
    }

    private static bool HasValidCpfCheckDigits(string digits)
    {
        // Sequencias como 111.111.111-11 passam no calculo, mas nao sao CPFs.
        if (digits.Distinct().Count() == 1)
            return false;

        var first = CheckDigit(digits, 9, 10);
        var second = CheckDigit(digits, 10, 11);

        return digits[9] == first && digits[10] == second;

        static char CheckDigit(string source, int length, int startWeight)
        {
            var sum = 0;

            for (var i = 0; i < length; i++)
                sum += (source[i] - '0') * (startWeight - i);

            var remainder = sum % 11;

            return (char)('0' + (remainder < 2 ? 0 : 11 - remainder));
        }
    }

    private static bool HasValidCnpjCheckDigits(string digits)
    {
        if (digits.Distinct().Count() == 1)
            return false;

        int[] firstWeights = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] secondWeights = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var first = CheckDigit(digits, firstWeights);
        var second = CheckDigit(digits, secondWeights);

        return digits[12] == first && digits[13] == second;

        static char CheckDigit(string source, int[] weights)
        {
            var sum = 0;

            for (var i = 0; i < weights.Length; i++)
                sum += (source[i] - '0') * weights[i];

            var remainder = sum % 11;

            return (char)('0' + (remainder < 2 ? 0 : 11 - remainder));
        }
    }

    /// <summary>Documento com pontuacao, para exibicao.</summary>
    public string ToFormattedString() => Type switch
    {
        CustomerType.Individual =>
            $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}",
        CustomerType.Company =>
            $"{Value[..2]}.{Value[2..5]}.{Value[5..8]}/{Value[8..12]}-{Value[12..]}",
        _ => Value,
    };

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
