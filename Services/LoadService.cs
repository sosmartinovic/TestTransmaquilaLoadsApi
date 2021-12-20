using transmaquila_loads_api.Models;
namespace transmaquila_loads_api.Services;

public static class LoadService{

    static List<Load> Loads{get;}

    static LoadService(){
        Loads = new List<Load>();
    }

    public static List<Load> GetAll(){
        //return Loads;
        return LoadFunction.GetDataCollection<Load>("GetAllLoads");
    }

    public static Load? Get(int id){

        return LoadFunction.SelectRecord<Load>("id",id,"tbl_loads");

    }

    public static void Add(Load load){

        System.Collections.ArrayList excludefields= new System.Collections.ArrayList();
        excludefields.Add("deleted");
        excludefields.Add("id");
        excludefields.Add("created_at");

        LoadFunction.AddRecord<Load>(ref load,"id","AddLoad", excludefields);

    }

    public static bool Update(Load load){

        return LoadFunction.UpdateRecord<Load>(ref load,"id","tbl_loads");

    }

    public static bool Delete(int id){
        return LoadFunction.DeleteRecord<Load>("id", id,"tbl_loads");
    }
}