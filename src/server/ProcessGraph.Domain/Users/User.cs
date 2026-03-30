using ProcessGraph.Domain.Abstractions;

namespace ProcessGraph.Domain.Users;

public class User : Entity
{
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }

    private User(Guid id, FirstName firstName, LastName lastName, Email email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }


    private User()
    {
    }


    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        return new User(Guid.NewGuid(), firstName, lastName, email);
    }
}