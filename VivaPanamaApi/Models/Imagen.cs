using System.ComponentModel.DataAnnotations;

public class imagen
{
    [Key]
    public int id_foto { get; set; }          

    public int id_lugar { get; set; }        

    public string url { get; set; }         
    public string descripcion { get; set; }  
}
