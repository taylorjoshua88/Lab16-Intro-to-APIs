using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoDbContext _context;

        public TodoController(TodoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.TodoItems.ToListAsync());
        }

        [HttpGet("{id:int}", Name = "GetTodo")]
        public async Task<IActionResult> GetById(int? id)
        {
            try
            {
                return Ok(await _context.TodoItems.SingleAsync(i => i.Id == id));
            }
            catch (Exception)
            {
                // TODO: Insert logging here
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TodoItem item)
        {
            if (item is null)
            {
                return BadRequest();
            }

            await _context.AddAsync(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // TODO: Insert logging here
                return StatusCode(StatusCodes.Status500InternalServerError, item);
            }

            return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
        }
    }
}