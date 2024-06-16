namespace BolWallet.Models;

public record CodenameExistsCheck
{
    private static readonly CodenameExistsCheck s_codenameDoesNotExist = new();
    
    private CodenameExistsCheck()
    { }
    
    public bool Exists { get; init; }
    public IEnumerable<string> Alternatives { get; init; } = [];

    public static CodenameExistsCheck CodenameDoesNotExist() => s_codenameDoesNotExist;

    public static CodenameExistsCheck CodenameExists(IEnumerable<string> alternatives)
    {
        if (alternatives == null || !alternatives.Any())
        {
            throw new ArgumentException("No alternatives provided", nameof(alternatives));
        }
        
        return new CodenameExistsCheck { Exists = true, Alternatives = alternatives };
    }
}
