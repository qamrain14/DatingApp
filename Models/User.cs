namespace datingApp.api.Models{
public class User{
    public int Id {get;set;}
    public string UserName {get;set;}

    public byte[] passwordHash{get;set;}
    public byte[] passwordSalt{get;set;}
}

}
