using transmaquila_loads_api.Models;
using transmaquila_loads_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace transmaquila_loads_api.Controllers;

[ApiController]
[Route("[controller]")]
public class LoadController:ControllerBase{

    public LoadController(){

    }

    [HttpGet]
    public ActionResult<List<Load>> GetAll() => LoadService.GetAll();

    [HttpPost]
    public IActionResult Create(Load load){
        LoadService.Add(load);
        return CreatedAtAction(nameof(Create), new {id=load.id}, load);
    }

    [HttpPut]
    public IActionResult Update(Load load){
        
        var LoadExist= LoadService.Get(load.id);
        if(LoadExist is not null){
            if(LoadExist.id ==0)
                return NotFound();

            LoadService.Update(load);
        }
        

        return NoContent();

    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id){
        var LoadExist= LoadService.Get(id);
        if(LoadExist is not null){
            if(LoadExist.id ==0)
                return NotFound();        
        }

        LoadService.Delete(id);

        return NoContent();
    }

}