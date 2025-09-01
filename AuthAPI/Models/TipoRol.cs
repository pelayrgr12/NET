namespace AuthAPI.Models;
public class TipoRol
{
    public int Id { get; set; }
    public string NombreRol { get; set; }
    public ICollection<Usuario> Usuarios { get; set; }


}