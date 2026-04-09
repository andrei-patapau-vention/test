using Microsoft.AspNetCore.Mvc;
using TestApi.Api.Models;

namespace TestApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private static readonly List<Item> Items =
    [
        new Item { Id = 1, Name = "Widget", Description = "A small widget" },
        new Item { Id = 2, Name = "Gadget", Description = "A useful gadget" },
        new Item { Id = 3, Name = "Doohickey", Description = null },
    ];

    private static readonly Lock ItemsLock = new();

    [HttpGet]
    public IEnumerable<Item> GetAll()
    {
        lock (ItemsLock)
            return Items.ToList();
    }

    [HttpGet("{id:int}")]
    public ActionResult<Item> GetById(int id)
    {
        lock (ItemsLock)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            return item is null ? NotFound() : Ok(item);
        }
    }

    [HttpPost]
    public ActionResult<Item> Create(Item item)
    {
        lock (ItemsLock)
        {
            item.Id = Items.Count > 0 ? Items.Max(x => x.Id) + 1 : 1;
            Items.Add(item);
        }
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Item> Update(int id, Item item)
    {
        lock (ItemsLock)
        {
            var existing = Items.FirstOrDefault(x => x.Id == id);
            if (existing is null)
                return NotFound();

            existing.Name = item.Name;
            existing.Description = item.Description;
            return Ok(existing);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        lock (ItemsLock)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item is null)
                return NotFound();

            Items.Remove(item);
            return NoContent();
        }
    }
}
