import subprocess
from datetime import date, timedelta

today = date(2026, 5, 25)

for d in range(0, 8):
    dt = today + timedelta(days=d)
    date_str = dt.strftime("%Y-%m-%d")
    sql = f"""
DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES (1, 2, 1, '{date_str}', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
"""
    result = subprocess.run(
        ["sqlcmd", "-S", ".", "-d", "Hospital", "-U", "sa", "-P", "123456", "-C", "-Q", sql],
        capture_output=True, text=True
    )
    errors = [l for l in result.stdout.split('\n') if l.strip() and '受影响' not in l and l.strip() != '(1 rows affected)']
    if not errors or all('受影响' in e for e in errors):
        print(f"  OK {date_str}")
    else:
        print(f"  FAIL {date_str}: {result.stdout[:200]}")
