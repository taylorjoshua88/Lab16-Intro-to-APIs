using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;

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
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.TodoItems.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int? id)
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
    }
}