namespace Globomantics.Domain;

public record Bug(string Title, 
    string Description, 
    Severity Serverity,
    int AffectedUsers,
    User CreatedBy,
    User? Assignedto,
    IEnumerable<byte[]> Images) : TodoTask(Title, DateTimeOffset.MinValue, CreatedBy);
