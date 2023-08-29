using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiDemo;
using Microsoft.AspNetCore.Authorization;

namespace app.ApiDemo
{
    [Route("api/vehicle")]
    [ApiController]
    [Authorize]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleContext _context;

        public VehicleController(VehicleContext context)
        {
            _context = context;
        }

        // GET: api/Vehicle
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        {
            if (_context.Vehicles == null)
            {
                return NotFound();
            }
            return await _context.Vehicles.ToListAsync();
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetMyVehicles()
        {
            if (_context.Vehicles == null)
            {
                return NotFound();
            }
            return await _context.Vehicles.Where(v => v.Owner == User!.Identity!.Name).ToListAsync();
        }
        // GET: api/Vehicle/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(string id)
        {
            if (_context.Vehicles == null)
            {
                return NotFound();
            }
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return vehicle;
        }

        // POST: api/Vehicle
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}/lock")]
        public async Task<ActionResult<Vehicle>> LockVehicle(string id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return Conflict();
            }

            vehicle.Locked = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/unlock")]
        public async Task<ActionResult<Vehicle>> UnlockVehicle(string id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return Conflict();
            }

            vehicle.Locked = false;
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("{id}/start")]
        public async Task<ActionResult<Vehicle>> StartVehicle(string id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return Conflict();
            }

            vehicle.Locked = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/stop")]
        public async Task<ActionResult<Vehicle>> StopVehicle(string id)
        {
            var vehicle = await GetOwnedVehicle(id);

            if (vehicle == null)
            {
                return Conflict();
            }

            vehicle.Locked = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<Vehicle?> GetOwnedVehicle(string vin) {
            var vehicle = await _context.Vehicles.FindAsync(vin);
            var currentUser = User?.Identity?.Name;
            if (vehicle == null)
                return null;

            if (string.IsNullOrWhiteSpace(vehicle.Owner))
                return null;

            if (vehicle.Owner == currentUser)
                return vehicle;

            return null;
        }

    }
}
