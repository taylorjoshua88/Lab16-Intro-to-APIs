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
            // RFC 2616 Section 10.2.1 recommends 200 response for successful
            // GET responses containing the requested resource data in the body.
            // If there are no items then this will simply create an empty JSON array
            return Ok(await _context.TodoItems.ToListAsync());
        }

        [HttpGet("{id:int}", Name = "GetTodo")]
        public async Task<IActionResult> GetById(int? id)
        {
            try
            {
                // RFC 2616 Section 10.2.1 recommends 200 response for successful
                // GET responses containing the requested resource data in the body
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

                // The new entity could not be committed to the database.
                // RFC 2616 Section 10.5 recommends that an explanation to be provided
                // for 500 responses.
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not commit new entity to the database");
            }

            return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, [FromBody] TodoItem item)
        {
            if (item is null || item.Id != id)
            {
                // RFC 2616 Section 10.4 recommends a plaintext explanation for 400 reponses
                return BadRequest("Entity's id field does not match request's id.");
            }

            TodoItem existingItem;

            try
            {
                existingItem = await _context.TodoItems.FirstAsync(i => i.Id == id);
            }
            catch
            {
                // RFC 2616 Section 10.4.5 (404)
                return NotFound();
            }

            // Update the existing item from the database without touching its Id
            existingItem.Title = item.Title;
            existingItem.IsComplete = item.IsComplete;

            _context.Update(existingItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // The update could not be committed to the database.
                // RFC 2616 Section 10.5 recommends that an explanation to be provided
                // for 500 responses.
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Unable to commit updated entity to backend database");
            }

            // RFC 2616 Section 10.2.5 (204)
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            TodoItem item;

            try
            {
                item = await _context.TodoItems.FirstAsync(i => i.Id == id);
            }
            catch
            {
                // Couldn't find the entity to delete; just return 404
                return NotFound();
            }

            _context.Remove(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Couldn't commit the delete; send a 500 response
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Unable to commit entity deletion to database.");
            }

            // RFC 2616 Section 9.7 states that DELETE with no entity included within
            // the response should use the 204 code.
            return NoContent();
        }
    }
}