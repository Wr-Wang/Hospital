using Hospital.Application.Repositories;
using Hospital.Domain.Aggregates.Schedule;
using Hospital.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfScheduleRepository : IScheduleRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfScheduleRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Schedule?> GetByIdAsync(long id)
        => await _db.Schedules.Include(s => s.Slots).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<Schedule>> GetByDoctorAsync(long doctorId)
        => await _db.Schedules.Include(s => s.Slots)
            .Where(s => s.DoctorId == doctorId)
            .ToListAsync();

    public async Task<List<Schedule>> GetByDeptAsync(long deptId, DateOnly? date)
    {
        var query = _db.Schedules.Include(s => s.Slots).Where(s => s.DeptId == deptId);
        if (date.HasValue)
            query = query.Where(s => s.ScheduleDate == date.Value);
        return await query.ToListAsync();
    }

    public async Task<List<Schedule>> GetAvailableAsync(long deptId, long? doctorId, DateOnly date)
    {
        var query = _db.Schedules.Include(s => s.Slots)
            .Where(s => s.DeptId == deptId
                && s.ScheduleDate == date
                && s.Status == ScheduleStatus.已发布
                && s.Slots.Any(sl => sl.TotalQuota - sl.BookedQuota > 0));
        if (doctorId.HasValue)
            query = query.Where(s => s.DoctorId == doctorId.Value);
        return await query.ToListAsync();
    }

    public async Task AddAsync(Schedule schedule)
    {
        await _db.Schedules.AddAsync(schedule);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Schedule schedule)
    {
        _db.Schedules.Update(schedule);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Schedules.Include(s => s.Slots).FirstOrDefaultAsync(s => s.Id == id);
        if (entity is not null)
        {
            _db.Schedules.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
