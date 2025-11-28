using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public abstract class CrudController<T>(AppDbContext context) : ODataController
    where T : class
{
    protected readonly AppDbContext _context = context;

    // -------------------------------------------------------
    // GET (collection)
    // -------------------------------------------------------
    [EnableQuery]
    public virtual IQueryable<T> Get()
        => _context.Set<T>().AsNoTracking();


    // -------------------------------------------------------
    // GET (single by key)
    // -------------------------------------------------------
    [EnableQuery]
    public virtual SingleResult<T> Get([FromODataUri] int key)
    {
        var result = _context.Set<T>()
            .AsNoTracking()
            .Where(e => EF.Property<int>(e, "Id") == key);

        return SingleResult.Create(result);
    }


    // -------------------------------------------------------
    // POST (create)
    // -------------------------------------------------------
    public virtual async Task<IActionResult> Post([FromBody] T entity, CancellationToken ct)
    {
        if (entity == null)
            return BadRequest("Request body is missing.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await OnBeforeCreate(entity);

        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync(ct);

        return Created(entity);
    }


    // -------------------------------------------------------
    // PATCH
    // -------------------------------------------------------
    public virtual async Task<IActionResult> Patch(
        [FromODataUri] int key,
        [FromBody] Delta<T> patch,
        CancellationToken ct)
    {
        if (patch == null)
            return BadRequest("Request body is missing.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var set = _context.Set<T>();
        var entity = await set.FindAsync(new object[] { key }, ct);

        if (entity == null)
            return NotFound();

        // --- PROTECT ID FROM BEING PATCHED ---
        if (patch.GetChangedPropertyNames().Contains("Id"))
        {
            // reset attempted ID change to original entity ID
            var originalId = entity.GetType().GetProperty("Id")!.GetValue(entity);
            patch.TrySetPropertyValue("Id", originalId);
        }

        // Domain-specific validation hook
        await OnBeforeUpdate(entity, patch);

        // Apply patch safely
        patch.Patch(entity);

        await _context.SaveChangesAsync(ct);

        return Updated(entity);
    }


    // -------------------------------------------------------
    // DELETE
    // -------------------------------------------------------
    public virtual async Task<IActionResult> Delete([FromODataUri] int key, CancellationToken ct)
    {
        var set = _context.Set<T>();
        var entity = await set.FindAsync(new object[] { key }, ct);

        if (entity == null)
            return NotFound();

        await OnBeforeDelete(entity);

        set.Remove(entity);
        await _context.SaveChangesAsync(ct);

        return NoContent();
    }


    // -------------------------------------------------------
    // EXTENSION HOOKS (override per entity)
    // -------------------------------------------------------
    protected virtual Task OnBeforeCreate(T entity) => Task.CompletedTask;

    protected virtual Task OnBeforeUpdate(T entity, Delta<T> patch) => Task.CompletedTask;

    protected virtual Task OnBeforeDelete(T entity) => Task.CompletedTask;
}
