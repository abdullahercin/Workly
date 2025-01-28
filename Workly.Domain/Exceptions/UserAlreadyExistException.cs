namespace Workly.Domain.Exceptions
{
    public class UserAlreadyExistException(string email) : Exception($"{email} mail adresi zaten kullanılıyor.")
    {


    }
}
