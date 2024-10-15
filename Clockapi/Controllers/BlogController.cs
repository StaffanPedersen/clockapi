namespace ClockApi.Controllers;

using Contexts;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly ApiContext context;
    private readonly ILogger<BlogController> _logger;

    public BlogController(ApiContext _context, ILogger<BlogController> logger)
    {
        context = _context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<BlogPost>>> Get()
    {
        try
        {
            List<BlogPost> blogs = await context.BlogPosts.ToListAsync();
            if (blogs != null)
            {
                return Ok(blogs);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BlogPost>> Get(int id)
    {
        try
        {
            BlogPost? blog = await context.BlogPosts.FindAsync(id);
            if (blog != null)
            {
                
                return Ok(blog);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<BlogPost>> Post(BlogPost blog)
    {
        try
        {
            await context.BlogPosts.AddAsync(blog);
            await context.SaveChangesAsync();
            return Ok(blog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<BlogPost>> Put(int id, BlogPost blog)
    {
        try
        {
            if (id != blog.Id)
            {
                return BadRequest();
            }
            else
            {
                context.Entry(blog).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(blog);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<BlogPost>> Delete(int id)
    {
        try
        {
            BlogPost? blog = await context.BlogPosts.FindAsync(id);
            if (blog != null)
            {
                context.BlogPosts.Remove(blog);
                await context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "Internal server error");
        }
    }
}