using System;
namespace transmaquila_loads_api.Models;

public enum load_type{
    Tanker=1,
    Flatbed=2,
    DryBox=3
}
public class Load{
    public int id { get; set; }
    public string? vendor_name { get; set; }
    public int? id_load_type { get; set; }
    public DateTime? leg_date { get; set; }
    public DateTime?  created_at {get;set;}
    public int? deleted {get;set;}
    public int? created_by { get; set; }
    
}